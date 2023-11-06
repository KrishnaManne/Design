using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using wps.core;


internal class Program
{
    public StrategyFactory strategyFactory { get; set; }

    public Program()
    {
        strategyFactory = new StrategyFactory();
    }

    private static async Task Main(string[] args)
    {
        Console.WriteLine("Please provide the workflow definition file. It should be .xml or .json file.");

        var filePath = Console.ReadLine();

        // Specify the path of the XML file
        //string filePath = ".//test.xml";
        //string filePath = ".//test.json";

        await new Program().ExecuteWorkflowDefinition(filePath, new FileInfo(filePath).Extension.Equals(".xml"));
    }

    private async Task ExecuteWorkflowDefinition(string filePath, bool isXml)
    {
        // Create a FileStream to read the XML file
        FileStream fs = new FileStream(filePath, FileMode.Open);

        try
        {
            WorkflowDefinition workflowDefinition = null;
            if (isXml)
            {
                // Create a XmlSerializer instance to deserialize the XML data
                XmlSerializer serializer = new XmlSerializer(typeof(WorkflowDefinition));

                // Deserialize the XML data to an array of Book objects
                workflowDefinition = (WorkflowDefinition)serializer.Deserialize(fs);

            }
            else
            {
                workflowDefinition = JsonSerializer.Deserialize<WorkflowDefinition>(fs, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            if (workflowDefinition == null || workflowDefinition.Workflows == null || workflowDefinition.Workflows.Count == 0)
            {
                Console.WriteLine("No workflow objects");
                return;
            }

            await ExecuteWorkflows(workflowDefinition.Workflows);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            fs.Close();
        }
    }

    public async Task ExecuteWorkflows(List<Workflow> workflows)
    {
        Dictionary<string, Workflow> workflowLookup = workflows.ToDictionary(w => w.Name);

        HashSet<string> executedWorkflows = new HashSet<string>();

        foreach (var workflow in workflows)
        {
            await ExecuteWorkflow(workflowLookup, executedWorkflows, workflow.Name);
        }
    }

    private async Task ExecuteWorkflow(Dictionary<string, Workflow> workflowLookup, HashSet<string> executedWorkflows, string workflowName)
    {
        if (!workflowLookup.ContainsKey(workflowName) || executedWorkflows.Contains(workflowName))
        {
            return;
        }

        Workflow workflow = workflowLookup[workflowName];

        if (!string.IsNullOrEmpty(workflow.RunAfter))
        {
            ExecuteWorkflow(workflowLookup, executedWorkflows, workflow.RunAfter);
        }

        Console.WriteLine($"Executing workflow: {workflow.Name}");

        Dictionary<string, Activity> activityLookup = workflow.Activities.ToDictionary(a => a.Name);
        HashSet<string> executedActivities = new HashSet<string>();

        foreach (var activity in workflow.Activities)
        {
            await ExecuteActivity(activityLookup, executedActivities, activity.Name);
        }

        executedWorkflows.Add(workflow.Name);
    }

    private async Task ExecuteActivity(Dictionary<string, Activity> activityLookup, HashSet<string> executedActivities, string activityName)
    {
        if (!activityLookup.ContainsKey(activityName) || executedActivities.Contains(activityName))
        {
            return;
        }

        Activity activity = activityLookup[activityName];

        if (!string.IsNullOrEmpty(activity.RunAfter))
        {
            ExecuteActivity(activityLookup, executedActivities, activity.RunAfter);
        }


        activity.Strategy = StrategyFactory
        .CreateStrategyInstance(activity.Type)
        .SetActivity(activity)
        .Validate()
        .Initialize();        

        try
        {
            Console.WriteLine($"Executing activity: {activity.Name} of type: {activity.Type}");
            await activity.ExecuteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Rolling back activity: {activity.Name} of type: {activity.Type}");
            await activity.RollbackAsync();
        }

        // Add logic to execute the specific activity type based on activity.Type and activity.Parameters
        executedActivities.Add(activity.Name);
    }
}