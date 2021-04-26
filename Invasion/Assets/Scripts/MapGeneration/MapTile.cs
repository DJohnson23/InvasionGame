using UnityEngine;


// Corner: Top/Bottom Right/Left
// Edge: Vertical/Horizontal Positive/Negative
public enum MapTileRuleArea
{
	TopLeft = 6, // 110
	TopMiddle = 3, // 011
	TopRight = 7, // 111
	LeftMiddle = 0, // 000
	RightMiddle = 1, // 001
	BottomLeft = 4, // 100
	BottomMiddle = 2, // 010
	BottomRight = 5  // 101
}

public enum MapTileRule
{
	None = 0,
	Full = 1,
	Empty = 2
}

public enum MapTileSelectionType
{
	Constant,
	RandomFromList
}

[System.Serializable]
public class MapTile
{
	[SerializeField]
	public MapTileSelectionType selectionType;
	[SerializeField]
	public int constTextureIndex;
	[SerializeField]
	public int[] randomList = new int[0];
	[SerializeField]
	public float[] randomWeight = new float[0];
	[SerializeField]
	public int orientation;
	[SerializeField]
	MapTileRule[] ruleSet = new MapTileRule[8];

	public MapTileRule CheckRule(MapTileRuleArea ruleArea)
	{
		return ruleSet[(int)ruleArea];
	}

	public void SetRule(MapTileRuleArea ruleArea, MapTileRule rule)
	{
		ruleSet[(int)ruleArea] = rule;
	}

	public int GetRandomTileIndex()
	{
		float totalWeight = 0;

		foreach (float weight in randomWeight)
		{
			totalWeight += weight;
		}

		float val = Random.Range(0, totalWeight);
		float sum = 0;

		for (int i = 0; i < randomWeight.Length; i++)
		{
			sum += randomWeight[i];

			if (sum > val)
			{
				return randomList[i];
			}
		}

		return randomList[randomList.Length - 1];
	}

	public int GetPriority(Texture2D map, int x, int y, bool drawBackWalls = true)
	{
		int priority = 0;
		for (int i = 0; i < 8; i++)
		{
			MapTileRule curRule = ruleSet[i];

			if (ruleSet[i] == MapTileRule.None)
			{
				continue;
			}

			int curX;
			int curY;

			//Corner
			if ((i & 4) == 4)
			{
				curY = y + ((i & 2) == 2 ? 1 : -1);
				curX = x + ((i & 1) == 1 ? 1 : -1);
			}
			//Edge Horizontal
			else if ((i & 2) == 2)
			{
				curX = x;
				curY = y + ((i & 1) == 1 ? 1 : -1);
			}
			//Edge Vertical
			else
			{
				curY = y;
				curX = x + ((i & 1) == 1 ? 1 : -1);
			}

			Color pixel;
			if(curX < 0 || curX > map.width - 1)
			{
				pixel = Color.white;
			}
			else if(curY < 0 || curY > map.height - 1)
			{
				if(drawBackWalls)
				{
					pixel = Color.white;
				}
				else
				{
					pixel = Color.black;
				}
			}
			else
			{
				pixel = map.GetPixel(curX, curY);
			}

			if (pixel == Color.black)
			{
				if (curRule == MapTileRule.Full)
				{
					priority++;
				}
				else
				{
					priority--;
				}
			}
			else if (curRule == MapTileRule.Empty)
			{
				priority++;
			}
			else
			{
				priority--;
			}
		}

		return priority;
	}

}