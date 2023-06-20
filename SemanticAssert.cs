using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

public class SemanticAssert
{
    private readonly ISKFunction _function;
    public SemanticAssert(IKernel sk, string function)
    {
        _function = sk.CreateSemanticFunction(function,
            "evaluate",
            "assertionskill",
            maxTokens: 4096);
    }

    public async Task IsSemanticallyCorrect(string input, string aiGeneratedMessage)
    {
        var contextVariables = new ContextVariables(input);
        contextVariables["aiGeneratedMessage"] = aiGeneratedMessage;

        var fnResult = await _function.InvokeAsync(new SKContext(contextVariables));

        if (fnResult.ErrorOccurred)
        {
            throw new Exception(fnResult.LastErrorDescription);
        }

        var result = JsonSerializer.Deserialize<SemanticAssertionResult>(fnResult.Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if(result.Result == false)
        {
            throw new Exception($"{result.Reason} Suggestion: {result.Suggestion}");
        }
    }
}