using System;

namespace Main
{
    // main logic class
    class Program
    {
        // initzation method; for use untill switching to actual devcade IDE
        // run via console: [dotnet run --project Main]
        public static void Main(String[]  args)
        {
            Initalize();
        }

        // does things which need to happen before loading content
        static void Initalize()
        {
            LoadContent;
        }

        // loads content
        static void LoadContent()
        {
            Update;
        }

        // primary update loop
        static void Update()
        {
            string line = Console.ReadLine();

            if (line == "test")
            {
                return;
            }

            Update();
        }
    }

    // handles individual units
    class Unit
    {
        public String name;
        public Unit(String name)
        {
            this.name = name;
        }
    }
}