﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Corner: Top/Bottom Right/Left
// Edge: Vertical/Horizontal Positive/Negative
public enum MapTileRuleArea
{
	TopLeft			= 6, // 110
	TopMiddle		= 3, // 011
	TopRight		= 7, // 111
	LeftMiddle		= 0, // 000
	RightMiddle		= 1, // 001
	BottomLeft		= 4, // 100
	BottomMiddle	= 2, // 010
	BottomRight		= 5  // 101
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
	public MapTileSelectionType selectionType;
	public GameObject constantPrefab;
	public GameObject[] randomList = new GameObject[0];
	public float[] randomWeight = new float[0];
	public float extraRotation;
	MapTileRule[] ruleSet = new MapTileRule[8];

	public MapTileRule CheckRule(MapTileRuleArea ruleArea)
	{
		return ruleSet[(int)ruleArea];
	}

	public void SetRule(MapTileRuleArea ruleArea, MapTileRule rule)
	{
		ruleSet[(int)ruleArea] = rule;
	}

	public GameObject GetRandomTile()
	{
		float totalWeight = 0;

		foreach(float weight in randomWeight)
		{
			totalWeight += weight;
		}

		float val = Random.Range(0, totalWeight);
		float sum = 0;

		for(int i = 0; i < randomWeight.Length; i++)
		{
			sum += randomWeight[i];

			if(sum > val)
			{
				return randomList[i];
			}
		}

		return randomList[randomList.Length - 1];
	}

	public int GetPriority(Texture2D map, int x, int y)
	{
		int priority = 0;
		for(int i = 0; i < 8; i++)
		{
			MapTileRule curRule = ruleSet[i];

			if(ruleSet[i] == MapTileRule.None)
			{
				continue;
			}

			int curX;
			int curY;
			//Corner
			if((i & 4) == 4)
			{
				curY = y + ((i & 2) == 2 ? 1 : -1);
				curX = x + ((i & 1) == 1 ? 1 : -1);
			}
			//Edge Vertical
			else if((i & 2) == 2)
			{
				curX = x;
				curY = y + ((i & 1) == 1 ? 1 : -1);
			}
			//Edge Horizontal
			else
			{
				curY = y;
				curX = x + ((i & 1) == 1 ? 1 : -1);
			}

			if(curX < 0 || curX > map.width - 1 || curY < 0 || curY > map.height - 1)
			{
				continue;
			}

			Color pixel = map.GetPixel(curX, curY);

			if(pixel == Color.black)
			{
				if(curRule == MapTileRule.Full)
				{
					priority++;
				}
				else
				{
					priority--;
				}
			}
			else if(curRule == MapTileRule.Empty)
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