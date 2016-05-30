# DbRepoProvider
DB Repository Provider

Database Repository Helper with UnitOfWork back-end implementation.

- .Net Framework version: **4.5**

- Requirements: 
  * **^EntityFramework 6.1.3**
  * **Oracle DB**

# Usage:

1. Add reference to DBRepoProvider.dll
2. Start using your helper:
  ```C#
        static void Main(string[] args)
        {
            using (var context = new Entities())
            {
                //Initialize your dbRepoProvider
                var dbRepoProvider = new UnitOfWork<Entities>(context: context, autoCommit: false);

                //Example with Linq
                var x = from r in dbRepoProvider.Context.TABLE
                        select r;

                //CRUD
                var table = dbRepoProvider.GetRepository<TABLE>().GetByID(1);

                //Procedure
                var proc = dbRepoProvider.ExecuteProcedure<CXT_TYPE>("PROCEDURE", arg1, arg2, arg3, ...);

                //Function
                var func = dbRepoProvider.ExecuteFunction("FUNCTION", arg1, arg2, arg3, ...);
            }
        }
  ```

