﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Terraria.ModLoader.UI
{
	internal class UIModConfigFloatItem : UIElement
	{
		private Color _color;
		private Func<string> _TextDisplayFunction;
		private Func<float> _GetProportion;
		private Action<float> _SetProportion;
		private Func<float> _GetValue;
		private Action<float> _SetValue;
		private int _sliderIDInPage;
		float min = 0;
		float max = 1;
		float increment = 0.01f;
		private PropertyFieldWrapper variable;
		private ModConfig modConfig;

		public UIModConfigFloatItem(PropertyFieldWrapper variable, ModConfig modConfig, int sliderIDInPage)
		{
			this.variable = variable;
			this.modConfig = modConfig;
			Width.Set(0f, 1f);
			Height.Set(0f, 1f);
			this._color = Color.White;
			this._TextDisplayFunction = () => variable.Name + ": " + _GetValue();
			this._GetValue = () => DefaultGetValue();
			this._SetValue = (float value) => DefaultSetValue(value);
			this._sliderIDInPage = sliderIDInPage;

			LabelAttribute att = (LabelAttribute)Attribute.GetCustomAttribute(variable.MemberInfo, typeof(LabelAttribute));
			if (att != null)
			{
				this._TextDisplayFunction = () => att.Label + ": " + _GetValue();
			}

			FloatValueRangeAttribute floatValueRange = (FloatValueRangeAttribute)Attribute.GetCustomAttribute(variable.MemberInfo, typeof(FloatValueRangeAttribute));
			FloatValueIncrementesAttribute floatValueIncrements = (FloatValueIncrementesAttribute)Attribute.GetCustomAttribute(variable.MemberInfo, typeof(FloatValueIncrementesAttribute));
			if (floatValueRange != null)
			{
				max = floatValueRange.Max;
				min = floatValueRange.Min;
			}
			if (floatValueIncrements != null)
			{
				increment = floatValueIncrements.increment;
			}
			this._GetProportion = () => DefaultGetProportion();
			this._SetProportion = (float proportion) => DefaultSetProportion(proportion);
		}

		void DefaultSetValue(float value)
		{
			variable.SetValue(modConfig, Utils.Clamp(value, min, max));
			Interface.modConfig.SetPendingChanges();
		}

		float DefaultGetValue()
		{
			return (float)variable.GetValue(modConfig);
		}

		float DefaultGetProportion()
		{
			return (_GetValue() - min) / (max - min);
		}

		void DefaultSetProportion(float proportion)
		{
			_SetValue((float)Math.Round((proportion*(max-min) + min) * (1 / increment)) * increment);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			float num = 6f;
			base.DrawSelf(spriteBatch);
			int num2 = 0;
			IngameOptions.rightHover = -1;
			if (!Main.mouseLeft)
			{
				IngameOptions.rightLock = -1;
			}
			if (IngameOptions.rightLock == this._sliderIDInPage)
			{
				num2 = 1;
			}
			else if (IngameOptions.rightLock != -1)
			{
				num2 = 2;
			}
			CalculatedStyle dimensions = base.GetDimensions();
			float num3 = dimensions.Width + 1f;
			Vector2 vector = new Vector2(dimensions.X, dimensions.Y);
			bool flag = false;
			bool flag2 = base.IsMouseHovering;
			if (num2 == 1)
			{
				flag2 = true;
			}
			if (num2 == 2)
			{
				flag2 = false;
			}
			Vector2 baseScale = new Vector2(0.8f);
			Color color = flag ? Color.Gold : (flag2 ? Color.White : Color.Silver);
			color = Color.Lerp(color, Color.White, flag2 ? 0.5f : 0f);
			Color color2 = flag2 ? this._color : this._color.MultiplyRGBA(new Color(180, 180, 180));
			Vector2 vector2 = vector;
			Utils.DrawSettingsPanel(spriteBatch, vector2, num3, color2);
			vector2.X += 8f;
			vector2.Y += 2f + num;
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, this._TextDisplayFunction(), vector2, color, 0f, Vector2.Zero, baseScale, num3, 2f);
			vector2.X -= 17f;
			Main.colorBarTexture.Frame(1, 1, 0, 0);
			vector2 = new Vector2(dimensions.X + dimensions.Width - 10f, dimensions.Y + 10f + num);
			IngameOptions.valuePosition = vector2;
			float obj = IngameOptions.DrawValueBar(spriteBatch, 1f, this._GetProportion(), num2);
			if (IngameOptions.inBar || IngameOptions.rightLock == this._sliderIDInPage)
			{
				IngameOptions.rightHover = this._sliderIDInPage;
				if (PlayerInput.Triggers.Current.MouseLeft && IngameOptions.rightLock == this._sliderIDInPage)
				{
					this._SetProportion(obj);
				}
			}
			if (IngameOptions.rightHover != -1 && IngameOptions.rightLock == -1)
			{
				IngameOptions.rightLock = IngameOptions.rightHover;
			}
		}
	}
}
