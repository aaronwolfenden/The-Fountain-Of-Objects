
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
        Map.SetRoomType(new Location(0, 0), RoomType.CavernEntrance);
        Map.SetRoomType(new Location(0, 1), RoomType.Empty);

        while (true)
        {
            Console.WriteLine("---------------------------------------");
            Console.WriteLine($"You are in the room at (Row={Player.Location.Row}, Column={Player.Location.Column})");

            if (Map.GetRoomType(Player.Location) == RoomType.Empty)
            {
                Console.WriteLine("This room is empty!");
            }
            if (Map.GetRoomType(Player.Location) == RoomType.CavernEntrance)
            {
                Console.WriteLine("You see light in this room coming from outside the cavern. This is the entrance.");
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
            Location newLocation = new Location(currentPlayerLocation.Row - 1, currentPlayerLocation.Column);
            if (newLocation.Row < 0)
            {
                Console.WriteLine("There is a wall here!");
            }
            else
            {
                game.Player.Location = newLocation;
            } 
        }


        if (Direction == Direction.South)
        {
            Location newLocation = new Location(currentPlayerLocation.Row + 1, currentPlayerLocation.Column);
            if (newLocation.Row > 3)
            {
                Console.WriteLine("There is a wall here!");
            }
            else
            {
                game.Player.Location = newLocation;
            }
        }


        if (Direction == Direction.West)
        {
            Location newLocation = new Location(currentPlayerLocation.Row, currentPlayerLocation.Column - 1);
            if (newLocation.Column < 0)
            {
                Console.WriteLine("There is a wall here!");
            }
            else
            {
                game.Player.Location = newLocation;
            }
        }


        if (Direction == Direction.East)
        {
            Location newLocation = new Location(currentPlayerLocation.Row, currentPlayerLocation.Column + 1);
            if(newLocation.Column > 3)
            {
                Console.WriteLine("There is a wall here!");
            }
            else
            {
                game.Player.Location = newLocation;
            }
        }


    }

}




// Enumeration to store the different types of room, allows for expanding upon
public enum RoomType { Empty, CavernEntrance, }

// Public enumeration to store a direction as it will be used in multiple instances
public enum Direction { North, South, East, West }