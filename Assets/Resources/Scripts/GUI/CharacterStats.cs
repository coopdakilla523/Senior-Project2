using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class CharacterStats : MonoBehaviour
{
	public PlayerBase player;
	public RawImage healthBar;
	private float healthBarWidth;
	public RawImage manaBar;
	private float manaBarWidth;
	public RawImage potion;
	private static List<Texture> potionImages = null;
	public Text scoreText;

	public void Start()
	{
		if(healthBar)
		{
			healthBarWidth = healthBar.rectTransform.rect.width;
		}
		if(manaBar)
		{
			manaBarWidth = manaBar.rectTransform.rect.width;
		}
		if(potionImages == null && potion != null)
		{
			potionImages = new List<Texture>();
			foreach(String pt in Enum.GetNames(typeof(PotionType)))
			{
				Texture tex = Resources.Load("Textures/GUI/Potions/" + pt.ToString().ToLower()) as Texture;
				if(!tex)
				{
					int w = (int) potion.rectTransform.rect.width;
					int h = (int) potion.rectTransform.rect.height;
					tex = new Texture2D(w, h);
				}
				potionImages.Add(tex);
			}
		}
	}


	public void LateUpdate()
	{
		if(player)
		{
			ResizeHealthBar(player.health/player.maxHealth);
			ResizeManaBar(player.mana/player.maxMana);
			DisplayPotion(player.item);
			UpdateScore(player.score);
		}
	}

	private void ResizeBar(RawImage bar, float barWidth, float percent)
	{
		if(bar)
		{
			float width = barWidth * percent;
			if(width <= 0)
			{
				//setting the width to 0.0f would cause an error
				width = 0.1f;
			}
			Rect rect = bar.rectTransform.rect;
			rect.width = width;

			Vector2 vec = new Vector2(width, bar.rectTransform.sizeDelta.y);
			bar.rectTransform.sizeDelta = vec;
		}
	}

	public void ResizeHealthBar(float percent)
	{
		ResizeBar(healthBar, healthBarWidth, percent);
	}

	public void ResizeManaBar(float percent)
	{
		ResizeBar(manaBar, manaBarWidth, percent);
	}

	public void DisplayPotion(PotionType type)
	{
		potion.texture = potionImages[(int)type];
//		switch(type)
//		{
//		case PotionType.NONE:
//			break;
//		case PotionType.HEALTH:
//			break;
//		case PotionType.HASTE:
//			break;
//		case PotionType.BIG:
//			break;
//		case PotionType.ATTACK:
//			break;
//		}
	}

	public void UpdateScore(int score)
	{
		scoreText.text = score + " Gold";
	}
}
