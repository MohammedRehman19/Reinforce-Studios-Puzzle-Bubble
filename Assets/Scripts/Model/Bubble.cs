using UnityEngine;
using System.Collections;
using System;

namespace com.javierquevedo{
	public enum BubbleColor {Red, Blue, Yellow, Green, Black, White};
	public class Bubble {
		
		private BubbleColor _color;
		
		public Bubble(BubbleColor color){
			this._color = color;
			
		}
		
		public BubbleColor color{
			get{
				return this._color;
			}
			set {
				this._color = value;
			}
		}

        public static implicit operator GameObject(Bubble v)
        {
            throw new NotImplementedException();
        }
    }
}
