
// Instantiate a new 3x3 map
using System.Security.Cryptography.X509Certificates;

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

    private readonly ISense[] _senses;

    public bool isFountainActivated { get; set; }

    public TheFountainOfObjectsGame(Map map, Player player)
    {
        Map = map;
        Player = player;

        // New senses can be added to this list as the game is expanded upon, allowing for scalability
        _senses = new ISense[]
        {
            new CavernEntranceSense(),
            new FountainSense()
        };
    }
    public void Run()
    {
        Map.SetRoomType(new Location(0, 0), RoomType.CavernEntrance);
        Map.SetRoomType(new Location(0, 1), RoomType.Empty);
        Map.SetRoomType(new Location(0, 2), RoomType.FountainRoom);

        while (!IsWon)
        {
            DialogueHelper.WriteLine("---------------------------------------", ConsoleColor.White);
            DialogueHelper.WriteLine($"You are in the room at (Row={Player.Location.Row}, Column={Player.Location.Column})", ConsoleColor.White);

            foreach(ISense sense in _senses)
            {
                if (sense.CanSense(this)) sense.Display(this);
            }


            DialogueHelper.Write("Which direction do you wish to move? (North, South, East, West) - ", ConsoleColor.Cyan);
            string? command = Console.ReadLine();
            ICommand playerCommand = command?.ToLower() switch
            {
                "move north" => new MoveCommand(Direction.North),
                "move south" => new MoveCommand(Direction.South),
                "move east" => new MoveCommand(Direction.East),
                "move west" => new MoveCommand(Direction.West),
                "enable fountain" => new ActivateCommand()
            };

            playerCommand.Execute(this);

            if (IsWon)
            {
                DialogueHelper.WriteLine("The Fountain Of Objects has been reactivated, and you have escaped with your life!", ConsoleColor.Magenta);
                DialogueHelper.WriteLine("You Win!", ConsoleColor.Magenta);
            }

        }
    }

    public bool IsWon => isFountainActivated && Map.GetRoomType(Player.Location) == RoomType.CavernEntrance;

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

    // Set the room type to a given room
    public void SetRoomType(Location location, RoomType room) => _rooms[location.Row, location.Column] = room;


    // Get the room type at the given location
    public RoomType GetRoomType(Location location) => _rooms[location.Row, location.Column];

    // Checks to see if the room exists on the map
    public bool RoomExists(Location location) => (location.Row < _rooms.GetLength(0) && location.Row >= 0 && location.Column < _rooms.GetLength(1) && location.Column >= 0);


}
// An interface for commands which further commands such as movement will be based off
public interface ICommand
{
    
    public void Execute(TheFountainOfObjectsGame game) { }
}


// A move command class based off the ICommand interface
public class MoveCommand : ICommand
{
    public Direction Direction { get; }

    public MoveCommand(Direction direction)
    {
        Direction = direction;
    }

    // Executes the movement command
    public void Execute(TheFountainOfObjectsGame game)
    {
        
        Location currentPlayerLocation = game.Player.Location;
        Location newLocation = Direction switch
        {
            Direction.North => new Location(currentPlayerLocation.Row - 1, currentPlayerLocation.Column),
            Direction.South => new Location(currentPlayerLocation.Row + 1, currentPlayerLocation.Column),
            Direction.East => new Location(currentPlayerLocation.Row, currentPlayerLocation.Column + 1),
            Direction.West => new Location(currentPlayerLocation.Row, currentPlayerLocation.Column - 1)
        };

        if (game.Map.RoomExists(newLocation))
        {
            game.Player.Location = newLocation;
        }
        else
        {
            DialogueHelper.WriteLine("There is a wall here.", ConsoleColor.White);
        }

    }

}
// An activation command for the fountain based off ICommand
public class ActivateCommand : ICommand
{
    public void Execute(TheFountainOfObjectsGame game)
    {
        // Checks to see if the fountain is in the same room as player and responds accordingly
        if (game.Map.GetRoomType(game.Player.Location) == RoomType.FountainRoom) game.isFountainActivated = true;
        else DialogueHelper.WriteLine("The fountain is not in this room.", ConsoleColor.White);
    }
}

// Interface for sensing which individual senses will be based off
public interface ISense
{
    // Check to see if anything can be sensed
    public bool CanSense(TheFountainOfObjectsGame game);

    // Displays the appropriate message if CanSense returns true
    public void Display(TheFountainOfObjectsGame game);
}

// A sense for checking if the player is at the cavern entrance
public class CavernEntranceSense : ISense
{
    // Check if the cavern entrance can be sensed
    public bool CanSense(TheFountainOfObjectsGame game) => game.Map.GetRoomType(game.Player.Location) == RoomType.CavernEntrance;
    // Displays a message if it can be sensed
    public void Display(TheFountainOfObjectsGame game) => DialogueHelper.WriteLine("You see light in this room coming from outside the cavern. This is the entrance.",
                                                            ConsoleColor.Yellow);

}
// A sense for checking if the player can sense the fountain 
public class FountainSense : ISense
{
    // Checks if the fountain is in the player's room
    public bool CanSense(TheFountainOfObjectsGame game) => game.Map.GetRoomType(game.Player.Location) == RoomType.FountainRoom;

    // Displays an appropriate message if the fountain is on or off
    public void Display(TheFountainOfObjectsGame game)
    {
        if (!game.isFountainActivated) DialogueHelper.WriteLine("You hear water dripping in this room. The Fountain Of Objects is here!", ConsoleColor.Blue);
        else DialogueHelper.WriteLine("You hear the rushing waters from The Fountain Of Objects. It has been reactivated!", ConsoleColor.Blue);
    }
}

// Static helper class which can be referenced for highlighting relevant messages within the game
public static class DialogueHelper
{
    // WriteLine method replacement for Console.WriteLine()
    public static void WriteLine(string text, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.WriteLine(text);
    }

    // Write method replacement for Console.Write()
    public static void Write(string text, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.Write(text);
    }
}




// Enumeration to store the different types of room, allows for expanding upon
public enum RoomType { Empty, CavernEntrance, FountainRoom}

// Public enumeration to store a direction as it will be used in multiple instances
public enum Direction { North, South, East, West }