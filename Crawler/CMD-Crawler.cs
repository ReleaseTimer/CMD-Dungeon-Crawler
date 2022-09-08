using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Crawler
{
    /**
     * The main class of the Dungeon Crawler Application
     * 
     * You may add to your project other classes which are referenced.
     * Complete the templated methods and fill in your code where it says "Your code here".
     * Do not rename methods or variables which already exist or change the method parameters.
     * You can do some checks if your project still aligns with the spec by running the tests in UnitTest1
     * 
     * For Questions do contact us!
     */
    public class CMDCrawler
    {
        /**
         * use the following to store and control the next movement of the yser
         */
        public enum PlayerActions { NOTHING, NORTH, EAST, SOUTH, WEST, PICKUP, ATTACK, QUIT };
        private PlayerActions action;
        /**
         * Global Variables
         */
        private bool active = true;//tracks if the game is running
        private bool gameRunning = false;//Tracks if game has played
        private bool initSuccess = false;//Turn true if map is loaded.
        private char[][] originalMap; // Uninitialized  char array to  stores Original Map
        private string keyStroke = ""; //Uninitialized string variable
        private string currentMapPath = "";// Contains map name
        private string currentMapName = "";

        //Advanced Mode Global Variables
        private bool advanceModeOn = false;//Check if user has advacned mode on.
        private int coinCount = 0;//Holds amount of gold that user has picked up.
        private int playerHealth = 5;//Holds current player health
        private int monsterKilled = 0;//Counts how many monsters killed
        private bool onCointile = false;//Checks if the user intop of gold tile
        private bool coinPickedup = false;//Checks if the user has picked up the gold
        Dictionary<string, int[]> monsterPos = new Dictionary<string, int[]>();//Stores monsterPosition in dictionary
        private PlayerActions previousAction = PlayerActions.NOTHING;//Stores user previous actions
        private enum monsterAction {NORTH, EAST, SOUTH, WEST};//Monster actions        
        

        private Dictionary<string, int[]> MonsterPositions()
        {
            Dictionary<string,int[]> monsterPositions = new Dictionary<string,int[]>();// Store monster position into a dictionary 
            int monsterAmount = 0;
            //Code Below Loops through and finds all the monsters inside and maps
            for (int x = 0; x < GetCurrentMapState().Length; x++)
                for (int y = 0; y < GetCurrentMapState()[x].Length; y++)
                {
                    if (GetCurrentMapState()[x][y] == 'M')
                    {
                        int[] Pos = { x, y };
                        monsterPositions.Add($"Monster{monsterAmount}", Pos);//Saves monster position
                        monsterAmount++;
                    }
                }
            return monsterPositions;
        }


        /// <summary>
        /// - We generate a random input for the monster
        /// - Monsters movement is updated accordingly
        /// </summary>
        /// <returns></returns>
        private char[][] MonsterUpdate()
        {
            char[][] currentMap = GetCurrentMapState();//Loads current state of map
            monsterPos = MonsterPositions();
            for (int i = 0; i < monsterPos.Count; i++)
            {
                Random rand = new Random();
                int[] pos = monsterPos[$"Monster{i}"];              
                int randMovement = rand.Next(0, 4);//Generating random number for monsters movement

                //Checks if player is North,East,South and West of the monster for an attack, if player isn't in those tiles we move
                if (currentMap[pos[0] - 1][pos[1]] ==  '@' || currentMap[pos[0]][pos[1] - 1] == '@' || currentMap[pos[0] + 1][pos[1]] == '@' || currentMap[pos[0]][pos[1] + 1] == '@')
                {
                    playerHealth--;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Damage Taken");
                    Console.ResetColor();
                }
                else
                {
                    switch (randMovement)//Moves the monster depending on generated number, monster can't pass through coins,monster,walls and players
                    {
                        case (int)monsterAction.NORTH:
                            if (currentMap[pos[0] - 1][pos[1]] == '#' || currentMap[pos[0] - 1][pos[1]] == 'M' || currentMap[pos[0] - 1][pos[1]] == 'C' || currentMap[pos[0] - 1][pos[1]] == 'X')//Checks position coins any of these chars
                            {
                                //Monsters don't Move
                            }
                            else
                            {
                                currentMap[pos[0]][pos[1]] = '-';//Replaces old monster location with empty space
                                currentMap[pos[0] - 1][pos[1]] = 'M';//Moves monster north
                            }
                                break;
                        case (int)monsterAction.SOUTH:
                            if (currentMap[pos[0] + 1][pos[1]] == '#' || currentMap[pos[0] + 1][pos[1]] == 'M' || currentMap[pos[0] + 1][pos[1]] == 'C' || currentMap[pos[0] + 1][pos[1]] == 'X')//Checks position coins any of these chars
                            {
                                //Monsters don't Move

                            }
                            else
                            {
                                currentMap[pos[0]][pos[1]] = '-';
                                currentMap[pos[0] + 1][pos[1]] = 'M';
                            }
                            break;
                        case (int)monsterAction.EAST:
                            if (currentMap[pos[0]][pos[1] + 1] == '#' || currentMap[pos[0]][pos[1] + 1] == 'M' || currentMap[pos[0]][pos[1] + 1] == 'C' || currentMap[pos[0]][pos[1] + 1] == 'X')//Checks position coins any of these chars
                            {
                                //Monsters don't Move
                            }
                            else
                            {
                                currentMap[pos[0]][pos[1]] = '-';
                                currentMap[pos[0]][pos[1] + 1] = 'M';
                            }
                            break;
                        case (int)monsterAction.WEST:
                            if (currentMap[pos[0]][pos[1] - 1] == '#' || currentMap[pos[0]][pos[1] - 1] == 'M' || currentMap[pos[0]][pos[1] - 1] == 'C' || currentMap[pos[0]][pos[1] - 1] == 'X')//Checks position coins any of these chars
                            {
                                //Monsters don't Move
                            }
                            else
                            {
                                currentMap[pos[0]][pos[1]] = '-';
                                currentMap[pos[0]][pos[1] - 1] = 'M';
                            }
                            break;

                        default:
                            break;
                    }
                }
                
                
                
            }
            return new char[0][];
        }





        /**
         * Reads user input from the Console
         * 
         * Please use and implement this method to read the user input.
         * 
         * Return the input as string to be further processed
         * 
         */
        private string ReadUserInput()
        {
            string inputRead = string.Empty;

            
            if (initSuccess == false)//Checks if we have loaded the map
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Load a Map by Typing\n- load Simple.map \n- load Simple2.map \n- load Advanced.map");
                Console.ResetColor();
                inputRead = Console.ReadLine();

            }
            else if (gameRunning == false)//Checks if the user had started the game
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Advanced Mode On: {advanceModeOn}\n{currentMapName} has being loaded\nType Play to start or Advanced to turn on/off advanced features");
                Console.ResetColor();
                inputRead = Console.ReadLine();
            }
            else
            {
                if (advanceModeOn == true)//If advanced mode on we read the user keys without having to wait them to press enter.
                {
                    ConsoleKeyInfo keyStroke = Console.ReadKey();//Reads user key input
                    if (keyStroke.Key.ToString() == "Spacebar")//Checks if user is sending a space key
                    {
                        inputRead = " ";
                    }
                    else
                    {
                        inputRead = keyStroke.Key.ToString();
                    }
                }
                else
                { 
                    inputRead = Console.ReadLine();
                }

            }
            return inputRead;
        }

        /**
         * Processed the user input string
         * 
         * takes apart the user input and does control the information flow
         *  * initializes the map ( you must call InitializeMap)
         *  * starts the game when user types in Play
         *  * sets the correct playeraction which you will use in the Update
         *  
         *  DO NOT read any information from command line in here but only act upon what the method receives.
         */
        public void ProcessUserInput(string input)
        {

            if (initSuccess == false)//Checks if map has being initialized yet
            {
                InitializeMap(input);
            }
            else if (gameRunning == false)
            {
                if (input.ToLower() == "play") //Check if user is ready to play and causes update method to be active.
                {
                    gameRunning = true;
                }
                else if (input.ToLower() == "advanced")// Checks if user typed advanced mode to turn on advanced features.
                {
                    if (advanceModeOn == false)
                        advanceModeOn = true;
                    else advanceModeOn = false;
                }
            }
            else if (gameRunning == true)
            {
                keyStroke = input.ToUpper();
            }
        }

        /**
         * The Main Game Loop. 
         * It updates the game state.
         * 
         * This is the method where you implement your game logic and alter the state of the map/game
         * use playeraction to determine how the character should move/act
         * the input should tell the loop if the game is active and the state should advance
         * 
         * Returns true if the game could be updated and is ongoing
         */
        public bool Update(bool active)
        {
            bool working = false;
            if (gameRunning == true)
            {
                int[] playerPos = GetPlayerPosition();//Gets player current position
                int userKeyStroke = GetPlayerAction();//Gets current user input
                char[][] currentMap = GetCurrentMapState();//Loads current state of map
                monsterPos = MonsterPositions();//Gets all current Monster positions

                if (playerHealth > 1)
                {
                    Console.Clear();//Clears previous printed map with updated action
                    switch (userKeyStroke)//Check what user has inputed and goes that case
                    {

                        case (int)PlayerActions.NORTH:
                            if (currentMap[playerPos[0] - 1][playerPos[1]] == '#' || currentMap[playerPos[0] - 1][playerPos[1]] == 'M')//Will Check if the monster or a wall is in the postion we are moving
                            {
                                Console.WriteLine("Can't move there");
                                working = true;
                            }
                            else if (currentMap[playerPos[0] - 1][playerPos[1]] == '-')//If the space is an empty space, the user postion will be updated
                            {
                                if (advanceModeOn == true)
                                {
                                    if (coinPickedup == true)//Checks if the user picked up the coin
                                    {
                                        currentMap[playerPos[0]][playerPos[1]] = '-';//replaces the coin with empty space
                                        currentMap[playerPos[0] - 1][playerPos[1]] = '@';//updates player position
                                        onCointile = false;
                                        coinPickedup = false;
                                    }
                                    else if (onCointile == true)//If goldPickedUp is false, we just put the coin in the space that the user left
                                    {
                                        currentMap[playerPos[0]][playerPos[1]] = 'C';
                                        currentMap[playerPos[0] - 1][playerPos[1]] = '@';
                                        onCointile = false;
                                    }
                                    else
                                    {
                                        //if all is false just updates player movement towards the empty tile
                                        currentMap[playerPos[0]][playerPos[1]] = '-';
                                        currentMap[playerPos[0] - 1][playerPos[1]] = '@';
                                    }
                                }
                                else if (onCointile == true)//Checks if user was on that coin tile so coin
                                {
                                    currentMap[playerPos[0]][playerPos[1]] = 'C';
                                    currentMap[playerPos[0] - 1][playerPos[1]] = '@';
                                    onCointile = false;
                                }
                                else
                                {
                                    currentMap[playerPos[0]][playerPos[1]] = '-';
                                    currentMap[playerPos[0] - 1][playerPos[1]] = '@';
                                    working = true;
                                }

                            }
                            else if (currentMap[playerPos[0] - 1][playerPos[1]] == 'C')//If the position user is moving has a coin
                            {
                                onCointile = true;//We set user being on a coin true
                                currentMap[playerPos[0]][playerPos[1]] = '-';
                                currentMap[playerPos[0] - 1][playerPos[1]] = '@';

                            }
                            else if (currentMap[playerPos[0] - 1][playerPos[1]] == 'X')//Checks if user has entered the exit doors 
                            {
                                //Moves current player, stops the game loop and prints out user coin and monster killed count
                                currentMap[playerPos[0]][playerPos[1]] = '-';
                                currentMap[playerPos[0] - 1][playerPos[1]] = '@';
                                action = PlayerActions.QUIT;
                                active = false;
                                gameRunning = false;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"You Exited And Have Collected {coinCount} Coins, Killed {monsterKilled} Monsters");
                                Console.ResetColor();
                            }
                            break;


                        case (int)PlayerActions.SOUTH:
                            if (currentMap[playerPos[0] + 1][playerPos[1]] == '#' || currentMap[playerPos[0] + 1][playerPos[1]] == 'M')
                            {
                                Console.WriteLine("Can't move there");
                                working = true;
                            }
                            else if (currentMap[playerPos[0] + 1][playerPos[1]] == '-')
                            {
                                if (advanceModeOn == true)
                                {
                                    if (coinPickedup == true)
                                    {
                                        currentMap[playerPos[0]][playerPos[1]] = '-';
                                        currentMap[playerPos[0] + 1][playerPos[1]] = '@';
                                        onCointile = false;
                                        coinPickedup = false;
                                    }
                                    else if (onCointile == true)
                                    {
                                        currentMap[playerPos[0]][playerPos[1]] = 'C';
                                        currentMap[playerPos[0] + 1][playerPos[1]] = '@';
                                        onCointile = false;
                                    }
                                    else
                                    {
                                        currentMap[playerPos[0]][playerPos[1]] = '-';
                                        currentMap[playerPos[0] + 1][playerPos[1]] = '@';
                                    }
                                }
                                else if (onCointile == true)
                                {
                                    currentMap[playerPos[0]][playerPos[1]] = 'C';
                                    currentMap[playerPos[0] + 1][playerPos[1]] = '@';
                                    onCointile = false;
                                }
                                else
                                {
                                    currentMap[playerPos[0]][playerPos[1]] = '-';
                                    currentMap[playerPos[0] + 1][playerPos[1]] = '@';
                                    working = true;
                                }

                            }
                            else if (currentMap[playerPos[0] + 1][playerPos[1]] == 'C')
                            {
                                onCointile = true;
                                currentMap[playerPos[0]][playerPos[1]] = '-';
                                currentMap[playerPos[0] + 1][playerPos[1]] = '@';
                            }
                            else if (currentMap[playerPos[0] + 1][playerPos[1]] == 'X')
                            {
                                currentMap[playerPos[0]][playerPos[1]] = '-';
                                currentMap[playerPos[0] + 1][playerPos[1]] = '@';
                                action = PlayerActions.QUIT;
                                active = false;
                                gameRunning = false;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"You Exited And Have Collected {coinCount} Coins, Killed {monsterKilled} Monsters");
                                Console.ResetColor();
                            }
                            break;

                        case (int)PlayerActions.EAST:
                            if (currentMap[playerPos[0]][playerPos[1] + 1] == '#' || currentMap[playerPos[0]][playerPos[1] + 1] == 'M')
                            {
                                Console.WriteLine("Can't move there");
                                working = true;
                            }
                            else if (currentMap[playerPos[0]][playerPos[1] + 1] == '-')
                            {
                                if (advanceModeOn == true)
                                {
                                    if (coinPickedup == true)
                                    {
                                        currentMap[playerPos[0]][playerPos[1]] = '-';
                                        currentMap[playerPos[0]][playerPos[1] + 1] = '@';
                                        onCointile = false;
                                        coinPickedup = false;
                                    }
                                    else if (onCointile == true)
                                    {
                                        currentMap[playerPos[0]][playerPos[1]] = 'C';
                                        currentMap[playerPos[0]][playerPos[1] + 1] = '@';
                                        onCointile = false;
                                    }
                                    else
                                    {
                                        currentMap[playerPos[0]][playerPos[1]] = '-';
                                        currentMap[playerPos[0]][playerPos[1] + 1] = '@';
                                    }
                                }
                                else if (onCointile == true)
                                {
                                    currentMap[playerPos[0]][playerPos[1]] = 'C';
                                    currentMap[playerPos[0]][playerPos[1] + 1] = '@';
                                    onCointile = false;
                                }
                                else
                                {
                                    currentMap[playerPos[0]][playerPos[1]] = '-';
                                    currentMap[playerPos[0]][playerPos[1] + 1] = '@';
                                    working = true;
                                }

                            }
                            else if (currentMap[playerPos[0]][playerPos[1] + 1] == 'C')
                            {
                                onCointile = true;
                                currentMap[playerPos[0]][playerPos[1]] = '-';
                                currentMap[playerPos[0]][playerPos[1] + 1] = '@';

                            }
                            else if (currentMap[playerPos[0]][playerPos[1] + 1] == 'X')
                            {
                                currentMap[playerPos[0]][playerPos[1]] = '-';
                                currentMap[playerPos[0]][playerPos[1] + 1] = '@';
                                action = PlayerActions.QUIT;
                                active = false;
                                gameRunning = false;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"You Exited And Have Collected {coinCount} Coins, Killed {monsterKilled} Monsters");
                                Console.ResetColor();
                            }
                            break;

                        case (int)PlayerActions.WEST:
                            if (currentMap[playerPos[0]][playerPos[1] - 1] == '#' || currentMap[playerPos[0]][playerPos[1] - 1] == 'M')
                            {
                                Console.WriteLine("Can't move there");
                                working = true;
                            }
                            else if (currentMap[playerPos[0]][playerPos[1] - 1] == '-')
                            {
                                if (advanceModeOn == true)
                                {
                                    if (coinPickedup == true)
                                    {
                                        currentMap[playerPos[0]][playerPos[1]] = '-';
                                        currentMap[playerPos[0]][playerPos[1] - 1] = '@';
                                        onCointile = false;
                                        coinPickedup = false;
                                    }
                                    else if (onCointile == true)
                                    {
                                        currentMap[playerPos[0]][playerPos[1]] = 'C';
                                        currentMap[playerPos[0]][playerPos[1] - 1] = '@';
                                        onCointile = false;
                                    }
                                    else
                                    {
                                        currentMap[playerPos[0]][playerPos[1]] = '-';
                                        currentMap[playerPos[0]][playerPos[1] - 1] = '@';
                                    }
                                }
                                else if (onCointile == true)
                                {
                                    currentMap[playerPos[0]][playerPos[1]] = 'C';
                                    currentMap[playerPos[0]][playerPos[1] - 1] = '@';
                                    onCointile = false;
                                }
                                else
                                {
                                    currentMap[playerPos[0]][playerPos[1]] = '-';
                                    currentMap[playerPos[0]][playerPos[1] - 1] = '@';
                                    working = true;
                                }

                            }
                            else if (currentMap[playerPos[0]][playerPos[1] - 1] == 'C')
                            {
                                onCointile = true;
                                currentMap[playerPos[0]][playerPos[1]] = '-';
                                currentMap[playerPos[0]][playerPos[1] - 1] = '@';
                            }
                            else if (currentMap[playerPos[0]][playerPos[1] - 1] == 'X')
                            {
                                currentMap[playerPos[0]][playerPos[1]] = '-';
                                currentMap[playerPos[0]][playerPos[1] - 1] = '@';
                                action = PlayerActions.QUIT;
                                active = false;
                                gameRunning = false;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"You Exited And Have Collected {coinCount} Coins, Killed {monsterKilled} Monsters");
                                Console.ResetColor();
                            }
                            break;

                        case (int)PlayerActions.ATTACK:
                            if (advanceModeOn == true)
                            {
                                switch(previousAction)//Checks where user was facing then attacks direction depending on his previous action
                                {
                                    case PlayerActions.NORTH:
                                        if(currentMap[playerPos[0] - 1][playerPos[1]] == 'M')
                                        {
                                            int[] mPos = {playerPos[0] - 1, playerPos[1] };//Stores monster location
                                            foreach (var monsterPosCheck in monsterPos)//Loops through monsters positions and checks which monster the player attacked
                                            {
                                                if (monsterPosCheck.Value == mPos)
                                                {
                                                    monsterPos.Remove(monsterPosCheck.Key);//removes monster from dictionary and map so we don't update that monster
                                                }
                                            }
                                            monsterKilled++;//Increaments monsters killed
                                            currentMap[playerPos[0] - 1][playerPos[1]] = '-';//replaces the monster that is killed with a empty space
                                            
                                        }
                                        break;

                                case PlayerActions.SOUTH:
                                        if(currentMap[playerPos[0] + 1][playerPos[1]] == 'M')
                                        {
                                            int[] mPos = {playerPos[0] + 1, playerPos[1] };
                                            foreach (var monsterPosCheck in monsterPos)
                                            {
                                                if (monsterPosCheck.Value == mPos)
                                                {
                                                    monsterPos.Remove(monsterPosCheck.Key);
                                                }
                                            }
                                            monsterKilled++;
                                            currentMap[playerPos[0] + 1][playerPos[1]] = '-';
                                            
                                        }
                                        break;

                                    case PlayerActions.EAST:
                                        if (currentMap[playerPos[0]][playerPos[1] + 1] == 'M')
                                        {
                                            int[] mPos = { playerPos[0], playerPos[1] + 1 };
                                            foreach (var monsterPosCheck in monsterPos)
                                            {
                                                if (monsterPosCheck.Value == mPos)
                                                {
                                                    monsterPos.Remove(monsterPosCheck.Key);
                                                }
                                            }
                                            monsterKilled++;
                                            currentMap[playerPos[0]][playerPos[1] + 1] = '-';

                                        }
                                        break;

                                    case PlayerActions.WEST:
                                        if (currentMap[playerPos[0]][playerPos[1] - 1] == 'M')
                                        {
                                            int[] mPos = { playerPos[0], playerPos[1] - 1 };
                                            foreach (var monsterPosCheck in monsterPos)
                                            {
                                                if (monsterPosCheck.Value == mPos)
                                                {
                                                    monsterPos.Remove(monsterPosCheck.Key);
                                                }
                                            }
                                            monsterKilled++;
                                            currentMap[playerPos[0]][playerPos[1] - 1] = '-';

                                        }
                                        break;

                                    default:
                                        break;
                                }

                            }

                            break;

                        case (int)PlayerActions.PICKUP:
                            if (advanceModeOn == true)
                            {
                                if (onCointile == true)//Checks is user is on gold tile
                                {
                                    coinCount++;//Increments gold count user has gained
                                    coinPickedup = true;
                                }
                                else
                                {
                                    Console.WriteLine("No coin here");//If they are not under gold it print this
                                }
                            }
                            break;

                        default:
                            break;
                    }

                    previousAction = action;//Stores previous action taken place.
                }
                else
                {
                    //If Players health goes down to zero, we stop game and display user death and his gold and monster killed count.
                    advanceModeOn = false;
                    action = PlayerActions.QUIT;
                    active = false;
                    gameRunning = false;
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.ResetColor();
                    Console.WriteLine($"You Died And Have Collected {coinCount} Coins, Killed {monsterKilled} Monsters");
                }

            }
            // Your code here

            return working;
        }

        /**
         * The Main Visual Output element. 
         * It draws the new map after the player did something onto the screen.
         * 
         * This is the method where you implement your the code to draw the map ontop the screen
         * and show the move to the user. 
         * 
         * The method returns true if the game is running and it can draw something, false otherwise.
        */
        public bool PrintMap()
        {
            if (gameRunning == true)//Checks that the game is running
            {
                //We print the map by looping through 2D array which holds the map
                for (int x = 0; x < GetCurrentMapState().Length; x++)
                {
                    Console.WriteLine();
                    for (int y = 0; y < GetCurrentMapState()[x].Length; y++)
                    {
                        Console.Write(GetCurrentMapState()[x][y]);
                    }
                }
                Console.WriteLine();
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * Additional Visual Output element. 
         * It draws the flavour texts and additional information after the map has been printed.
         * 
         * This is the method does not need to be used unless you want to output somethign else after the map onto the screen.
         * 
         */
        public bool PrintExtraInfo()
        {

            //Prints out user health, coin count and monsters killed
            if (advanceModeOn == true && gameRunning == true)
            {
                MonsterUpdate();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Coin Count: {coinCount}");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Player Health: {playerHealth}");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Monster Killed: {monsterKilled}");
                Console.ResetColor();


            }
            return true;
        }

        /**
        * Map and GameState get initialized
        * mapName references a file name 
        * Do not use abosolute paths but use the files which are relative to the executable.
        * 
        * Create a private object variable for storing the map in Crawler and using it in the game.
        */
        public bool InitializeMap(String mapName)
        {
            initSuccess = false;
            try
            {
                if (mapName.Contains("Simple.map"))//Checks if command from user contains Simple.map
                {

                    currentMapPath = "./maps/Simple.map";//We set currentMapPath to where the location of the file should be
                    initSuccess = File.Exists(currentMapPath);//Checks if we have files within the current path, returns true or false for initSuccess
                    currentMapName = "Simple.map";

                }
                else if (mapName.Contains("Simple2.map"))//Checks if command from user contains Simple2.map
                {

                    currentMapPath = "./maps/Simple2.map";
                    initSuccess = File.Exists(currentMapPath);//Checks if we have files within the current path, returns true or false for initSuccess
                    currentMapName = "Simple2.map";

                }
                else if (mapName.Contains("Advanced.map"))//Checks if command from user contains Advanced.map
                {

                    currentMapPath = "./maps/Advanced.map";
                    initSuccess = File.Exists(currentMapPath);//Checks if we have files within the current path, returns true or false for initSuccess
                    currentMapName = "Advanced.map";
                }
                else
                {
                    Console.WriteLine("Invalid Command - " + mapName);
                    initSuccess = File.Exists(currentMapPath);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("File is missing");//
                throw ex;
            }
            return initSuccess;
        }

        /**
         * Returns a representation of the currently loaded map
         * before any move was made.
         * This map should not change when the player moves
         */
        public char[][] GetOriginalMap()
        {
          
                string lines = "";
                List<string> loadMapRows = new List<string>();//List to contains the amount of rows we have for the map
            if (initSuccess == true)
            {
                using (StreamReader sR = new StreamReader(currentMapPath))//Readings the map file from currentMapPath
                {
                    while ((lines = sR.ReadLine()) != null)//Reads text file until null is returned
                    {
                        loadMapRows.Add(lines);
                    }

                    int rows = loadMapRows.Count;//Getting the numbers of rows for 2D array
                    char[][] map = new char[rows][];//Initilizing the rows into the 2D map array;

                    //Initilizing the number of columns per rows, depending on number of char
                    for (int i = 0; i < rows; i++)
                    {
                        map[i] = new char[loadMapRows[i].Length];
                    }


                    string mapInString = String.Join("", loadMapRows);//Joining all the lines read into one string
                    int mapCharIncreament = 0;//Holds which char values from mapInString we are loading into our 2D array

                    //Loops and loads each char value from mapInString to the correct part of the 2D array
                    for (int x = 0; x < map.Length; x++)
                        for (int y = 0; y < map[x].Length; y++)
                        {
                            if (mapInString[mapCharIncreament] == 'S')//Finds starting point of player and replaces with user icon
                            {
                                map[x][y] = '@';
                                mapCharIncreament++;
                            }
                            else
                            {
                                map[x][y] = mapInString[mapCharIncreament];
                                mapCharIncreament++;
                            }
                        }
                    return map;
                }
            }
            else 
            {
                return new char[0][];//Returns empty 2D array if InitSuccess if false
            }
        }

        /*
         * Returns the current map state and contains the player's move
         * without altering it 
         */
        public char[][] GetCurrentMapState()
        {
            //Checks if we have a map loadeds
            char[][] map;
            if (originalMap == null)//If originalMap is empty we load a map 
            {
                map = GetOriginalMap();//We load map
            }
            else
            {
                map = originalMap;
            }
            return map;
        }

        /**
         * Returns the current position of the player on the map
         * 
         * The first value is the y coordinate and the second is the x coordinate on the map
         */
        public int[] GetPlayerPosition()
        {
            originalMap = GetCurrentMapState();//We gets make sure the map is current loaded into originalMap 2d Array
            int[] position = { 0, 0 };//Contains player current position
            
            //We loops through the 2D array to find current position of the player.
            for (int x = 0; x < originalMap.Length; x++)
                for (int y = 0; y < originalMap[x].Length; y++)
                {
                    if (originalMap[x][y] == '@')
                    {
                        position[0] = x;
                        position[1] = y;
                        break;//Stops loop if player position is found
                    }
                }

            return position;
        }

        /**
        * Returns the next player action
        * 
        * This method does not alter any internal state
        */
        public int GetPlayerAction()
        {
            //Switch cases to read User KeyInput, returns nothing if any random key is pressed
            switch (keyStroke)
            {
                case "W":
                    action = PlayerActions.NORTH;
                    break;
                case "S":
                    action = PlayerActions.SOUTH;
                    break;

                case "D":
                    action = PlayerActions.EAST;
                    break;
                case "A":
                    action = PlayerActions.WEST;
                    break;
                case "P":
                    action = PlayerActions.PICKUP;
                    break;
                case " ":
                    action = PlayerActions.ATTACK;
                    break;
                case "quit":
                    action = PlayerActions.QUIT;
                    break;
                default:
                    action = PlayerActions.NOTHING;
                    break;
            }
            return (int)action;
        }


        public bool GameIsRunning()
        {
            
            if(gameRunning == true)
                return gameRunning = true;
            else return gameRunning = false;

        }

        /**
         * Main method and Entry point to the program
         * ####
         * Do not change! 
        */
        static void Main(string[] args)
        {
            CMDCrawler crawler = new CMDCrawler();

            string input = string.Empty;
            Console.WriteLine("Welcome to the Commandline Dungeon!" +Environment.NewLine+ 
                "May your Quest be filled with riches!"+Environment.NewLine);
            
            // Loops through the input and determines when the game should quit
            while (crawler.active && crawler.action != PlayerActions.QUIT)
            {
                Console.WriteLine("Your Command: ");
                input = crawler.ReadUserInput();
                Console.WriteLine(Environment.NewLine);

                crawler.ProcessUserInput(input); 
            
                crawler.Update(crawler.active);
                crawler.PrintMap();
                crawler.PrintExtraInfo();
            }

            Console.WriteLine("See you again" +Environment.NewLine+ 
                "In the CMD Dungeon! ");


        }


    }
}
