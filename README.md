# Introduction

The TestChecker framework adds the ability to run pre-defined tests via ASP.Net Core Middleware.  
These tests are "live" tests, so they can be used to help with regression testing in UAT or debugging "live" issues.   

The following Endpoints are created:  

* `/test` will return TestChecker results as json (**only when the correct ApiKey is provided**).  
* `/testui` will give you a UI to the TestChecks.
* `/testdata` used internally to transfer TestData.

**NOTE:** To be secure, you need to block external access to `/test`.

# Examples

## Configuring the Middleware

In its simplest form, with no dependencies - Only local tests are run:
```c#
app.UseTestEndpoint(null, () => new MyTestChecks());
```

With dependencies - The tests configured in the dependencies are run as well as the local tests:
```c#
app.UseTestEndpoint(new List<ITestCheckDependency> { 
    new TestCheckDependency(new WebApplicationClient1.Client("https://localhost:7254")),
    new TestCheckDependency(new WebApplicationClient2.Client("https://localhost:7291"))
}, () => new MyTestChecks());
```

## Configuring Tests

### Setting up TestData

Provide TestData, so you can paramaterise the tests and change details dynamically - **This is useful if using the endpoint to help with debugging**.

```c#
public class MyTestData
{
    public int EmployerId { get; set; }
}
```

### Implement ITestCheck

Create a class that implements `ITestCheck`, which you pass into `UseTestEndpoint`.  
This class should hit every method in the interface you want to test via the following functions:
* TestIsTrue
* TestIsTrueAsync
* TestIsObject
* TestIsObjectAsync

These tests will track which methods you have called and give you an idea of test coverage.   

**NOTE:** It's recommended that you use `TestIsObject/TestIsObjectAsync` as the output will be returned in the json and you can then use that as before/after test.   


```c#
class MyTestChecks : ITestChecks<MyTestData>
{
    private readonly ImyController _myController;
    private MyTestData _testData;
    
    public MyTestChecks(MyController myController, MyTestData testData)
    {
        _myController = myController;
        _testData = testData;
    }

    public MyTestData GetTestData() => _testData;
    public void SetTestData(MyTestData testData) => _testData = testData;

    public async Task<TestCheck> RunReadTestsAsync()
    {
        var allTests = new TestCheck("MyController Tests");

        var tests = new TestCheck<ImyController, MyTestData>(_myController, _testData, CoverageMethod.MethodsOnly, null);
        tests.TestIsObject(x => x.GetEmployer(_testData.EmployerId));

        allTests.Add(tests);

        return await Task.FromResult(allTests);
    }

    public Task<TestCheck> RunWriteTestsAsync()
    {
        throw new System.NotImplementedException();
    }
}
```

### Hitting /test

Hitting the `/test` endpoint will return the TestCheck results.  
If configured correctly, this will hit every controller endpoint, thus helping with regression.  
eg, compare the `/test` results from the current website with the new website.

```json
{
  "System": "WebApplicationChild, Version=2.2.1.1, Url=https://localhost:7291/test?apikey=read",
  "Success": true,
  "TestCoverage": {
    "Percentage": 100.0,
    "Detail": "[1 / 1]"
  },
  "ReadTestChecks": {
    "Description": "MyController Tests",
    "Success": true,
    "SuccessCount": 1,
    "Coverage": {
      "Percentage": 100.0,
      "Detail": "[1 / 1]"
    },
    "TestChecks": [
      {
        "Object": "ImyController",
        "Success": true,
        "SuccessCount": 1,
        "Coverage": {
          "Object": "ImyController",
          "CoverageMethod": "MethodsOnly",
          "Percentage": 100.0,
          "Detail": "[1 / 1]"
        },
        "TestChecks": [
          {
            "Method": "ImyController.GetEmployer(Int32 employerId)",
            "Parameters": "employerId=555(EmployerId)",
            "ReturnValue": "{\"Id\":555,\"Name\":\"MyEmployer\"}",
            "Success": true,
            "SuccessCount": 1
          }
        ]
      }
    ]
  },
  "TestData": [
    {
      "FullName": "WebApplicationOldChild.MyTestData",
      "TestData": {
        "EmployerId": 555
      }
    }
  ],
  "TestDate": "20/12/2022",
  "Environment": "Development"
}
```

### Configuring Dependencies
Clients need to implement `TestChecker.Core.ITestCheckable` or inherit from `TestChecker.Core.TestCheckable`