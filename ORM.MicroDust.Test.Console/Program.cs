using MicroDust.Core;
using ORM.MicroDust.Test.Console.FuncTest;
using ORM.MicroDust.Test.Console.Model;
using System;
using System.Collections.Generic;

namespace ORM.MicroDust.Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //User uTest = new User
            //{
            //    DtCreate = DateTime.Now,
            //    Name = "机器人3",
            //    ID = Guid.NewGuid().ToString(),
            //    Sex = false,
            //    Age = 0,
            //    Status = 2
            //};

            //int fRow = ORMDbContext.Singleton.Add<User>(uTest);

            //List<User> users = ORMDbContext.Singleton.FindAll<User>().OrderBy(t => t.Status, OrderStatus.DESC).ThenBy(t => t.DtCreate).ToList();

            //if (null != users)
            //{
            //    foreach (User u in users)
            //        System.Console.WriteLine($"{u.Name}-{u.Sex}-{u.DtCreate}");
            //}

            //ORMDbContext.Singleton.Delete<User>(t => t.Status == 2);

            //ORMDbContext.Singleton.Update<User>(new
            //{
            //    Name = "纷纷"
            //}, "5ae1e315-1f72-4455-bde4-bffd5f57253c");
            List<UserModel> users2 = new List<UserModel>();
            //using (DBSqlTransaction tran = ORMDbContext.Singleton.DBSqlTransaction)
            //{
            //    string g = Guid.NewGuid().ToString();

            //    User uTest1 = new User
            //    {
            //        DtCreate = DateTime.Now,
            //        Name = "机器人0",
            //        ID = g,
            //        Sex = false,
            //        Status = 3
            //    };
            //    User uTest2 = new User
            //    {
            //        DtCreate = DateTime.Now,
            //        Name = "机器人0",
            //        ID = Guid.NewGuid().ToString(),
            //        Sex = false,
            //        Status = 3
            //    };
            //    tran.Add<User>(uTest1);
            //    tran.Add<User>(uTest2);
            //    tran.Update<User>(new
            //    {
            //        Name = "纷纷"
            //    }, "80f73aaf-d299-49d8-ac55-68510bb5bd1e");

            //    tran.Execute();

            //    //if (users2.Count > 2)
            //    //    tran.Rollback();
            //}

            //int fRowCount = ORMDbContext.Singleton.Increment<User>(new
            //{
            //    Status = 3
            //}, "80f73aaf-d299-49d8-ac55-68510bb5bd1e");

            Guid uID = Guid.Parse("80f73aaf-d299-49d8-ac55-68510bb5bd1e");

            var v = ORMDbContext.Singleton.Find<UserModel>().GroupBy(t => new
            {
                t.Name,
                t.Sex,
                t.Status
            });
            users2 = v.ToList();
            if (null != users2)
            {
                foreach (UserModel u in users2)
                    System.Console.WriteLine($"{u.Name}-{u.Sex}-{u.Status}-{u.Age}-{u.DtCreate}");
            }

            System.Console.ReadLine();
        }

        #region FuncTest
        public static void FuncTest()
        {
            //System.Console.WriteLine(FuncSimple.FuncMath(3, 4).ToString());

            //System.Console.WriteLine(FuncSimple.FuncMath2<Test>(t =>
            //{
            //    return t.Value;
            //}, new Test
            //{
            //    Value = 9
            //}));

            //System.Console.WriteLine(FuncSimple.FuncMath3<Test>(t => t.Value = 10).Value);

            //System.Console.WriteLine(FuncSimple.FuncMath4<Test>(t => t.Value));

            System.Console.WriteLine(FuncSimple.FuncGetPropertyName<Test, int>(t => t.Value));
        }

        public class Test
        {
            public int Value { get; set; }
        }
        #endregion
    }
}
