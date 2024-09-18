using System;

Random random = new Random();
Console.CursorVisible = false;
int height = Console.WindowHeight - 1;
int width = Console.WindowWidth - 5;
bool shouldExit = false;

// Console position of the player
int playerX = 0;
int playerY = 0;

// Console position of the food
int foodX = 0;
int foodY = 0;

// Available player and food strings
string[] states = { "('-')", "(^-^)", "(X_X)" };
string[] foods = { "@@@@@", "$$$$$", "#####" };

// Current player string displayed in the Console
string player = states[0];

// Index of the current food
int food = 0;

// Speed of Player
int speed = 0;

InitializeGame();
while (!shouldExit)
{
    if (TerminalResized())
    {
        shouldExit = true;
        Console.Clear();
        Console.WriteLine("Console was resized. Program exiting.");
    }
    else
    {
        Move(true);
    }
}

// Clears the console, displays the food and player
void InitializeGame()
{
    Console.Clear();
    ShowFood();
    Console.SetCursorPosition(0, 0);
    Console.Write(player);
}

// Reads directional input from the Console and moves the player
void Move(bool checkForNondirectionalKeyInput = false)
{
    int lastX = playerX;
    int lastY = playerY;

    switch (Console.ReadKey(true).Key)
    {
        case ConsoleKey.UpArrow:
            playerY--;
            break;
        case ConsoleKey.DownArrow:
            playerY++;
            break;
        case ConsoleKey.LeftArrow:
            playerX = playerX - 1 - speed;
            break;
        case ConsoleKey.RightArrow:
            playerX = playerX + 1 + speed;
            break;
        case ConsoleKey.Escape:
            shouldExit = true;
            break;
        default:
            if (checkForNondirectionalKeyInput)
            {
                shouldExit = true;
                Console.Clear();
                Console.WriteLine("Nondirectional key input detected. Program exiting.");
                return;
            }
            break;
    }

    // Clear the characters at the previous position
    RemovePlayer(lastX, lastY);

    // Keep player position within the bounds of the Terminal window
    playerX = (playerX < 0) ? 0 : (playerX >= width ? width : playerX);
    playerY = (playerY < 0) ? 0 : (playerY >= height ? height : playerY);

    if (CheckPlayerAndFoodOverlap())
    {
        ChangePlayer();
        ShowFood();
        CheckToFreezePlayer();
    }
    else
    {
        // Draw the player at the new location
        Console.SetCursorPosition(playerX, playerY);
        Console.Write(player);
    }
}

// Returns true if the Terminal was resized 
bool TerminalResized()
{
    return height != Console.WindowHeight - 1 || width != Console.WindowWidth - 5;
}

// Displays random food at a random location
void ShowFood()
{
    RemoveFood();

    // Update food to a random index
    food = random.Next(0, foods.Length);

    // Update food position to a random location
    foodX = random.Next(0, width - player.Length);
    foodY = random.Next(0, height - 1);

    // Display the food at the location
    Console.SetCursorPosition(foodX, foodY);
    Console.Write(foods[food]);
}

// Removes the food string from the console
void RemoveFood()
{
    Console.SetCursorPosition(foodX, foodY);
    for (int i = 0; i < foods[food].Length; i++)
    {
        Console.Write(" ");
    }
}

// Clear the characters at the previous position
void RemovePlayer(int lastX, int lastY)
{
    Console.SetCursorPosition(lastX, lastY);
    for (int i = 0; i < player.Length; i++)
    {
        Console.Write(" ");
    }
}

// Changes the player to match the food consumed
void ChangePlayer()
{
    RemovePlayer(playerX, playerY);

    player = states[food];
    ChangeSpeed(true);

    Console.SetCursorPosition(playerX, playerY);
    Console.Write(player);
}


// Temporarily stops the player from moving
void FreezePlayer()
{
    System.Threading.Thread.Sleep(1000);
    player = states[0];
}

// Changes the speed based on the player's state
void ChangeSpeed(bool active)
{
    if (active)
    {
        if (player == "(^-^)")
        {
            speed += 3;
        }
        else
        {
            speed = 0;
        }
    }
}

bool CheckPlayerAndFoodOverlap()
{
    bool leftColumnCollision = ((playerX + 5) >= foodX);
    bool rightColumnCollision = (playerX <= (foodX + 5));
    bool topColumnCollision = ((playerY + 1) >= foodY);
    bool bottomColumnCollision = (playerY <= (foodY + 1));

    if (leftColumnCollision && rightColumnCollision
    && topColumnCollision && bottomColumnCollision)
    {
        return true;
    }
    else
    {
        return false;
    }
}

bool CheckToFreezePlayer()
{
    if (player == "(X_X)")
    {
        FreezePlayer();
        return true;
    }
    return false;
}
