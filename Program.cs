
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

    private readonly ISense[] _senses;

    public TheFountainOfObjectsGame(Map map, Player player)
    {
        Map = map;
        Player = player;

        // New senses can be added to this list as the game is expanded upon, allowing for scalability
        _senses = new ISense[]
        {
            new CavernEntranceSense()
        };
    }
    public void Run()
    {
        Map.SetRoomType(new Location(0, 0), RoomType.CavernEntrance);
        Map.SetRoomType(new Location(0, 1), RoomType.Empty);

        while (true)
        {
            Console.WriteLine("---------------------------------------");
            Console.WriteLine($"You are in the room at (Row={Player.Location.Row}, Column={Player.Location.Column})");

            foreach(ISense sense in _senses)
            {
                if (sense.CanSense(this)) sense.Display(this);
            }


            Console.Write("Which direction do you wish to move? (North, South, East, West) - ");
            string? command = Console.ReadLine();
            ICommand playerCommand = command?.ToLower() switch
            {
                "move north" => new MoveCommand(Direction.North),
                "move south" => new MoveCommand(Direction.South),
                "move east" => new MoveCommand(Direction.East),
                "move west" => new MoveCommand(Direction.West),
            };

            playerCommand.Execute(this);
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
            Console.WriteLine("There is a wall here.");
        }

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


public class CavernEntranceSense : ISense
{
    // Check if the Cavern exit can be sensed
    public bool CanSense(TheFountainOfObjectsGame game) => game.Map.GetRoomType(game.Player.Location) == RoomType.CavernEntrance;

    public void Display(TheFountainOfObjectsGame game) => Console.WriteLine("You see light in this room coming from outside the cavern. This is the entrance.");

}




// Enumeration to store the different types of room, allows for expanding upon
public enum RoomType { Empty, CavernEntrance, }

// Public enumeration to store a direction as it will be used in multiple instances
public enum Direction { North, South, East, West }