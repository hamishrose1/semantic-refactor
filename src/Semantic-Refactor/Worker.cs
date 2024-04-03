using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
namespace SemanticRefactor
{
#pragma warning disable SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    public class Worker : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly Kernel _kernel;

        public Worker(IHostApplicationLifetime hostApplicationLifetime,
        [FromKeyedServices("AngularConversionKernel")] Kernel kernel)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _kernel = kernel;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var problem = "You must upgrade an AngularJs application in directory ./content/ to AngularJs" +
                "Create A new angular 17 application in a new directory called 'nzb', then convert the javascript controllers and HTML pages to be components." +
                "Process every file and decide the action to take, whether it should become part of a new component, an existing component, be copied, be deleted" +
                "Use the angular CLI to scaffold new components, then edit them with the existing pages" +
                "You may also use the CLI to use other tools to upgrade the application" +
                "You are finished when all files in ./contenet/ have been processed and the application has been upgraded to Angular 17";

            var options = new FunctionCallingStepwisePlannerOptions
            {
                MinIterationTimeMs = 100,
                MaxIterations = 150,
                MaxTokens = 300000, //gpt3516k
            };
            var planner = new FunctionCallingStepwisePlanner(options);

            var result = await planner.ExecuteAsync(_kernel, problem);

            Console.WriteLine("Iterations: {0}", result.Iterations);
            
            foreach(var iteration in result.ChatHistory)
            {
                Console.WriteLine(iteration.Content);
            }

            _hostApplicationLifetime.StopApplication();
        }
    }
}
