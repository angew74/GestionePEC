using System;
using System.Collections;
using System.Drawing;

namespace Com.Delta.PrintManager.Engine
{
	/// <summary>
	/// Summary description for TextLine.
	/// </summary>
	internal class TextLine
	{
		private ArrayList segments = new ArrayList();
		internal TextField.TextAlignmentType Alignment = TextField.TextAlignmentType.Left;
		private float width = 0;
		private float height = 0;

		public TextLine()
		{
			
		}

		public float Width
		{
			get {return width;}
			set {width = value;}
		}

		public float Height
		{
			get {return height;}			
		}

		public void SetSegments(ArrayList segs, float defaultHeight)
		{
			this.segments = segs;			
			
			foreach(TextSegment segment in segments)
			{
				height = (float)Math.Max(height, segment.Height);				
			}

			if (height == 0)
				height = defaultHeight;
		}

		public ArrayList Segments
		{
			get {return segments;}
		}
	}
}
