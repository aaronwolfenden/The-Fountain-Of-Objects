
// Instantiate a new 3x3 map
Map map = new Map(3, 3);
// Instantiate a new start position for the player class
Location startLocation = new Location(0, 0);
// Instantiate a new player starting at 0,0
Player player = new Player(startLocation);


TheFountainOfObjectsGame game = new TheFountainOfObjectsGame(map, player);
game.Run();


public class TheFountainOfObjectsGame
{
    public Map Map { get; }
    public Player Player { get; }

    public TheFountainOfObjectsGame(Map map, Player player)
    {
        Map = map;
        Player = player;
    }
    public void Run()
    {
        while (true)
        {
            Console.WriteLine("---------------------------------------");
            Console.WriteLine($"You are in the room at (Row={Player.Location.Row}, Column={Player.Location.Column})");
            Console.Write("Which direction do you wish to move? (North, South, East, West) - ");
            string? command = Console.ReadLine();
            ICommand playerCommand = command?.ToLower() switch
            {
                "north" => new MoveCommand(Direction.North),
                "south" => new MoveCommand(Direction.South),
                "east" => new MoveCommand(Direction.East),
                "west" => new MoveCommand(Direction.West)
            };

            playerCommand.Execute(this);
            Console.WriteLine($"You are in the room at (Row={Player.Location.Row}, Column={Player.Location.Column})");
        }


    }

}


public class Player
{
    public Location Location { get; set; }

    public Player(Location location)
    {
        Location = location;
    }
}

public record Location(int Row, int Column);

public class Map
{
    private readonly RoomType[,] _rooms;
    public int Rows { get; set; }
    public int Columns { get; set; }

    public Map(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        _rooms = new RoomType[Rows, Columns];
    }

}

public interface ICommand
{
    // An interface for commands which further commands such as movement will be based off
    public void Execute(TheFountainOfObjectsGame game) { }
}

public class MoveCommand : ICommand
{
    // A move command class based off the ICommand interface
    public Direction Direction { get; }

    public MoveCommand(Direction direction)
    {
        Direction = direction;
    }

    public void Execute(TheFountainOfObjectsGame game)
    {
        // Executes the movement command
        Location currentPlayerLocation = game.Player.Location;
        if (Direction == Direction.North)
        {
            Location newLocation = new Location(currentPlayerLocation.Row + 1, currentPlayerLocation.Column);
            game.Player.Location = newLocation;
        }
        if (Direction == Direction.South)
        {
            Location newLocation = new Location(currentPlayerLocation.Row - 1, currentPlayerLocation.Column);
            game.Player.Location = newLocation;
        }
        if (Direction == Direction.West)
        {
            Location newLocation = new Location(currentPlayerLocation.Row, currentPlayerLocation.Column - 1);
            game.Player.Location = newLocation;
        }
        if (Direction == Direction.East)
        {
            Location newLocation = new Location(currentPlayerLocation.Row, currentPlayerLocation.Column + 1);
            game.Player.Location = newLocation;
        }


    }

}



// Enumeration to store the different types of room, allows for expanding upon
public enum RoomType { Empty, CavernEntrance, }

// Public enumeration to store a direction as it will be used in multiple instances
public enum Direction { North, South, East, West }