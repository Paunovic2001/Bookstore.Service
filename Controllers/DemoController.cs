using Microsoft.AspNetCore.Mvc;
using Rhetos;
using Rhetos.Processing;
using Rhetos.Host;
using Rhetos.Processing.DefaultCommands;
using Microsoft.OpenApi.Writers;

[Route("Demo/[action]")]
public class DemoController : ControllerBase
{
    private readonly IProcessingEngine processingEngine;
    private readonly IUnitOfWork unitOfWork;



    public DemoController(IRhetosComponent<IProcessingEngine> processingEngine, IRhetosComponent<IUnitOfWork> unitOfWork)
    {
        this.processingEngine = processingEngine.Value;
        this.unitOfWork = unitOfWork.Value;
    }

    [HttpGet]
    public string ReadBooks()
    {
        var readCommandInfo = new ReadCommandInfo { DataSource = "Bookstore.Book", ReadTotalCount = true };
        var result = processingEngine.Execute(readCommandInfo);
        return $"{result.TotalCount} books.";
    }

    //ne radi, ne isprobavaj na swaggeru, strga sve
    //radi kad se pogase row permissioni na Entity Book, trenutno su ugaseni
    [Route("Demo/[action]/{name}")]
    [HttpPost()]
    public string WriteBook(string name = "")
    {
        using (var scope = RhetosHost.CreateFrom(Path.Combine(Directory.GetCurrentDirectory(), @".\bin\Debug\net6.0\Bookstore.Service.dll")))
        {
            var context = scope.CreateScope().Resolve<Common.ExecutionContext>();
            var repository = context.Repository;

            Console.WriteLine(context.UserInfo.UserName);
            var employeeToInsert = repository.Bookstore.Employee.Query().FirstOrDefault(e => e.Name == name);

            var newBook = new Bookstore.Book { Title = "NewBook", EmployeeID = employeeToInsert.ID };
            var saveCommandInfo = new SaveEntityCommandInfo { Entity = "Bookstore.Book", DataToInsert = new[] { newBook } };
            processingEngine.Execute(saveCommandInfo);
            unitOfWork.CommitAndClose(); // Commits and closes database transaction.
            return "1 book inserted.";
        }
    }
}