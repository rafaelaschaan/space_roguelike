using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	// Using Serializable allows us to embed a class with sub properties in the inspector.
	[Serializable]
	public class Count
	{
		public int minimum;             //Minimum value for our Count class.
		public int maximum;             //Maximum value for our Count class.
		
		
		//Assignment constructor.
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}
	

	public int columns;                                         //Number of columns in our game board.
	public int rows;                                            //Number of rows in our game board.

	public GameObject[] chunkTiles;                                 //Array of floor prefabs.
	public GameObject[] outerWallTiles;                             //Array of outer tile prefabs.
	public GameObject[] enemies;
	public GameObject[] items;
	public GameObject key;

	float chunkWidth;
	float chunkHeight;

	private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
	private List <GameObject> gridPositions = new List <GameObject> ();   //A list of possible locations to place tiles.
	
	
	//Clears our list gridPositions and prepares it to generate a new board.
	void InitialiseList ()
	{
		//Clear our list gridPositions.
		gridPositions.Clear ();
		/**
		//Loop through x axis (columns).
		for(int x = 0; x < columns; x++)
		{
			//Within each column, loop through y axis (rows).
			for(int y = 0; y < rows; y++)
			{
				//At each index add a new Vector3 to our list with the x and y coordinates of that position.
				gridPositions.Add (new GameObject());
			}
		}
		**/
	}
	
	
	//Sets up the outer walls and floor (background) of the game board.
	void BoardSetup ()
	{
		//Instantiate Board and set boardHolder to its transform.
		boardHolder = new GameObject ("Board").transform;
		chunkWidth = chunkTiles [0].transform.GetChild(0).transform.localScale.x * 10f;
		chunkHeight = chunkTiles [0].transform.GetChild(0).transform.localScale.z * 10f;

		//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
		for(int x = -1; x < columns + 1; x++)
		{
			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
			for(int y = -1; y < rows + 1; y++)
			{
				//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
				GameObject toInstantiate;
				if(gridPositions.Count == 0 || gridPositions.Count == 1 || gridPositions.Count == rows || gridPositions.Count == rows+1)
					toInstantiate = chunkTiles[0];
				else{
				if(Random.Range(0,100) < 60)
					toInstantiate = chunkTiles[0];
				else
					toInstantiate = chunkTiles[Random.Range (1,chunkTiles.Length)];
				}
				//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
				if(x == -1 || x == columns || y == -1 || y == rows)
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
				
				//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
				GameObject instance =
					Instantiate (toInstantiate, new Vector3 (x*chunkWidth, y*chunkHeight, 5.0f), toInstantiate.transform.rotation) as GameObject;
				
				//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
				instance.transform.SetParent (boardHolder);
				instance.AddComponent("chunkController");
				instance.GetComponent<chunkController>().InitialiseList();
				instance.GetComponent<chunkController>().SetChunkRow(y);
				instance.GetComponent<chunkController>().SetChunkColumn(x);


				if(x != -1 && x != columns && y != -1 && y != rows){
					instance.SetActive(false);
					gridPositions.Add(instance);
				}
			}
		}
		//gridPositions [0].SetActive (true);
	}
	
	
	//RandomPosition returns a random position from our list gridPositions.
	GameObject RandomChunk ()
	{
		//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
		int randomIndex = Random.Range (0, gridPositions.Count);
		
		//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
		GameObject randomPosition = gridPositions[randomIndex];
		
		//Remove the entry at randomIndex from the list so that it can't be re-used.
		//gridPositions.RemoveAt (randomIndex);
		
		//Return the randomly selected Vector3 position.
		return randomPosition;
	}

	//SetupScene initializes our level and calls the previous functions to lay out the game board
	public void SetupScene (int level)
	{
		//Creates the outer walls and floor.
		BoardSetup ();
		
		//Reset our list of gridpositions.
		//InitialiseList ();

		//Remove the chunk where player starts the game and adjascents
		removeChunksFromList ();

		int randonChunks = Random.Range (gridPositions.Count / 4, Mathf.RoundToInt(gridPositions.Count / 1.5f));
		for (int i = 0; i < randonChunks; i++) {
			GameObject randChunk = RandomChunk();
			//SetupEnemies(randChunk);
			//SetupItems(randChunk);
		}
		randonChunks = Random.Range (gridPositions.Count / 4, Mathf.RoundToInt(gridPositions.Count / 1.5f));
		for (int i = 0; i < randonChunks; i++) {
			//GameObject randChunk = RandomChunk();
			//SetupItems(randChunk);
		}
		GameObject otherRandChunk = RandomChunk();
		SetupKey (otherRandChunk);


		//gameObject.AddComponent<NavMesh> ();

	}

	public void SetupEnemies(GameObject chunk){
		chunkController chunkcontrol = chunk.GetComponent<chunkController> ();
		List<Vector3> chunkPositions = chunkcontrol.gridPositions;
		int numEnemies = Random.Range (2, 6);
		for (int i = 0; i < numEnemies; i++) {
			GameObject toInstantiate = enemies[Random.Range (0,enemies.Length)];
			Vector3 randPos = chunkcontrol.RandomPosition();
			Vector3 instancePos = chunk.transform.position+randPos;
			instancePos.z = 0;
			Instantiate (toInstantiate,instancePos, Quaternion.identity);
		}
		
	}

	public void SetupItems(GameObject chunk){
		chunkController chunkcontrol = chunk.GetComponent<chunkController> ();
		List<Vector3> chunkPositions = chunkcontrol.gridPositions;
		int numItems = Random.Range (1, 3);
		for (int i = 0; i < numItems; i++) {
			GameObject toInstantiate = items[Random.Range (0,items.Length)];
			Vector3 randPos = chunkcontrol.RandomPosition();
			Vector3 instancePos = chunk.transform.position+randPos;
			instancePos.z = 0;
			Instantiate (toInstantiate,instancePos, Quaternion.identity);
		}
		
	}

	public void SetupKey(GameObject chunk){
		chunkController chunkcontrol = chunk.GetComponent<chunkController> ();
		List<Vector3> chunkPositions = chunkcontrol.gridPositions;

		GameObject toInstantiate = key;
		Vector3 randPos = chunkcontrol.RandomPosition();
		Vector3 instancePos = chunk.transform.position+randPos;
		instancePos.z = 0;
		Instantiate (toInstantiate,instancePos, Quaternion.identity);
		
	}

	void removeChunksFromList(){
		//gridPositions.RemoveAt (rows + 1);
		//gridPositions.RemoveAt (rows);
		//gridPositions.RemoveAt (1);
		//gridPositions.RemoveAt (0);
	}


	void VisualizeActiveRows(){
		int chunkRowIndex = 0;
		int chunkColumnIndex = 0;
		foreach (GameObject chunk in gridPositions) {
			if(chunk.GetComponent<chunkController>().hasPlayer){
				chunkRowIndex = chunk.GetComponent<chunkController>().chunkRow;
				chunkColumnIndex = chunk.GetComponent<chunkController>().chunkColumn;

				break;
			}
		}
		for(int i = 0; i < rows; i++){
			for(int j = 0; j < columns; j++){
				int index = i+j*columns;
				if((i >= chunkRowIndex -1 && i <= chunkRowIndex +1) && (j >= chunkColumnIndex -1 && j <= chunkColumnIndex +1))
					gridPositions[index].SetActive(true);
				else
					gridPositions[index].SetActive(false);
				}
			}
	}

	void Update(){
		//ShowOnlyChunksOnCamera ();
		VisualizeActiveRows ();
	}

}