using DatabaseBuilder;
using Neo4j.Driver;

public class AirportDataIngestor
{
    public static void Main()
    {
        //If true, will only load the first 300 airports and routes that connect them. Makes things way faster for testing.
        bool quickModeEnabled = false;
        if (quickModeEnabled)
        {
            Console.WriteLine("Running with quick mode enabled. Will only load the first 300 airports and routes that connect them.");
        }

        using var cypher = new CypherBindings("bolt://localhost:7687", "neo4j", "password");

        FlushDB(cypher);

        Console.WriteLine("Beginning DB rebuild...");

        int progressCounter;
        HashSet<string> safeIDs;

        AddAirportNodes(quickModeEnabled, cypher, out progressCounter, out safeIDs);

        progressCounter = AddRouteRelations(quickModeEnabled, cypher, safeIDs);

        Console.WriteLine("Database rebuild complete.");

        cypher.Validation(quickModeEnabled ? 300 : 7698, quickModeEnabled ? 1219 : 66771);

        Thread.Sleep(1000);
    }

    private static int AddRouteRelations(bool quickModeEnabled, CypherBindings cypher, HashSet<string> safeIDs)
    {
        int progressCounter;
        Console.WriteLine("Adding route relations...");
        Console.Write("                     ]\r[");
        progressCounter = 0;
        foreach (string line in File.ReadLines("./routes.csv"))
        {
            List<string> tokens = line.Replace("\"", "").Split(',').ToList(); //" Characters also removed from this input to make string formatting work.

            progressCounter++;
            if (progressCounter % (67663 / 20) == 0)
            {
                Console.Write("█");
            }

            if (tokens[3] == "\\N" || tokens[5] == "\\N") //Skip any records that don't have a valid airport ID for either their source or destination
                continue;

            if (quickModeEnabled && (!safeIDs.Contains(tokens[3]) || !safeIDs.Contains(tokens[5])))
                continue; //In safe mode, skips the slow step if the route isn't between two airports in the first 300

            cypher.CreateRoute(tokens);
            Thread.Sleep(5);
        }
        Console.WriteLine("]\nRoute relations added.");
        return progressCounter;
    }

    private static void AddAirportNodes(bool quickModeEnabled, CypherBindings cypher, out int progressCounter, out HashSet<string> safeIDs)
    {
        Console.WriteLine("Adding airport nodes...");
        Console.Write("                     ]\r[");
        progressCounter = 0;
        safeIDs = new HashSet<string>();
        foreach (string line in File.ReadLines("./airports.csv"))
        {
            List<string> tokens = line.Replace("\"", "").Split(',').ToList(); //All " characters are removed to avoid issues with string formatting later.
                                                                              //Accounts for the edge case in which airport names contain a comma, which splits the second entry into two.
            if (tokens.Count > 14)
            {
                tokens.Insert(1, $"{tokens[1]} {tokens[2]}");
                tokens.RemoveRange(2, 2);
            }
            cypher.CreateAirport(tokens);
            Thread.Sleep(5);

            progressCounter++;
            if (progressCounter % ((quickModeEnabled ? 300 : 7698) / 20) == 0)
            {
                Console.Write("█");
            }

            if (quickModeEnabled)
            {
                safeIDs.Add(tokens[0]);
            }

            if (quickModeEnabled && progressCounter >= 300)
                break; //Early exit for quick mode
        }
        Console.WriteLine("]\nAirport nodes added.");
    }

    private static void FlushDB(CypherBindings cypher)
    {
        Console.WriteLine("Flushing DB...");
        cypher.FlushDB();
        Console.WriteLine("DB flush completed.\n");

        Thread.Sleep(100);
    }
}