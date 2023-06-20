using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using System.Reflection;

var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
            .Build();

var embeddingConfig = configBuilder.GetRequiredSection("EmbeddingConfig").Get<Config>();
var completionConfig = configBuilder.GetRequiredSection("CompletionConfig").Get<Config>();

var sk = Kernel.Builder.Configure(embeddingConfig, completionConfig);

var simluatedAIValues = new[]
{
    "I did not find any results",
    "I found the following results: Item A: $100, Item B: $5, Item C: $1,000,000",
    "wombat",
    "I found the following results: Wallet: $5, Shoes: $10, Hat: $20",
    "Pair of pants for $1, $2 or $100",
    "Rusty nails for $20"
};

var semanticAssert = new SemanticAssert(sk, Assembly.GetEntryAssembly().LoadEmbeddedResource("sk-assert.skills.assertion.skprompt.txt"));

foreach (var item in simluatedAIValues)
{
    try
    {
        await semanticAssert.IsSemanticallyCorrect("Find me 3 items that cost less than $100", item); 
        
        Console.WriteLine($"[Pass] {item}");
    }
    catch(Exception e)
    {
        Console.WriteLine($"[Fail] {e.Message}");
    }    
}