
using System;
using System.Collections;
using System.Drawing;


namespace Com.Delta.PrintManager.Engine
{
	/// <summary>
	/// Class representing the Paper object.
	/// </summary>
	/// <remarks>Contains static methods related to Paper objects.</remarks>
	public class Paper
	{
		#region Declarations

		private static Hashtable types = new Hashtable();

		static Paper()
		{
			types.Add(Type.A0, new Size(3310,4680));
			types.Add(Type.A1, new Size(2338,3310));
			types.Add(Type.A2, new Size(1654,2338));
			types.Add(Type.A3, new Size(1169,1654));
			types.Add(Type.A4, new Size(827,1169));
			types.Add(Type.A5, new Size(583,827));
			types.Add(Type.A6, new Size(414,583));
			types.Add(Type.B0, new Size(3937,5568));
			types.Add(Type.B1, new Size(2784,3937));
			types.Add(Type.B2, new Size(1969,2784));
			types.Add(Type.B3, new Size(1390,1969));
			types.Add(Type.B4, new Size(985,1390));
			types.Add(Type.B5, new Size(693,985));
			types.Add(Type.B6, new Size(493,693));
			types.Add(Type.Letter, new Size(850,1100));
			types.Add(Type.LetterGovernment, new Size(809,1063));
			types.Add(Type.Legal, new Size(850,1400));
			types.Add(Type.Ledger, new Size(1111,1721));
			types.Add(Type.Quarto, new Size(809,1012));
			types.Add(Type.Executive, new Size(725,1050));
			types.Add(Type.EnvelopeMonarch, new Size(827,1169));
			types.Add(Type.EnvelopeC5, new Size(638,902));
			types.Add(Type.EnvelopeDL, new Size(433,866));
			types.Add(Type.Foolscap, new Size(809,1314));
			types.Add(Type.Statement, new Size(558,860));
		}
		/// <summary>
		/// Enumeration of possible paper type selections
		/// </summary>
		public enum Type
		{
			/// <summary>A0 paper (841x1189mm)</summary>
			A0=1,
			/// <summary>A1 paper (594x841mm)</summary>
			A1,
			/// <summary>A2 paper (1000x1414mm)</summary>
			A2,
			/// <summary>A3 paper (297x420mm)</summary>			
			A3,
			/// <summary>A4 paper (210x297mm)</summary>
			A4,
			/// <summary>A5 paper (148x210mm)</summary>
			A5,
			/// <summary>A6 paper (105x148mm)</summary>
			A6,
			/// <summary>B0 paper (707x1000mm)</summary>
			B0,
			/// <summary>B1 paper (176x250mm)</summary>
			B1,
			/// <summary>B2 paper (500x707mm)</summary>
			B2,
			/// <summary>B3 paper (353x500mm)</summary>
			B3,
			/// <summary>B4 paper (250x353mm)</summary>
			B4,
			/// <summary>B5 paper (176x250mm)</summary>
			B5,
			/// <summary>B6 paper (125x176mm)</summary>
			B6,
			/// <summary>Custom paper size</summary>
			Custom,
			/// <summary>EnvelopeC5 paper (162x229mm)</summary>
			EnvelopeC5,
			/// <summary>Envelope Paper (110x220 mm)</summary>
			EnvelopeDL,
			/// <summary>Envelope Monarch paper (98x190mm)</summary>
			EnvelopeMonarch,
			/// <summary>Executive paper (184x266mm)</summary>
			Executive,
			/// <summary>Foolscap Paper (8x13in)</summary>
			Foolscap,						
			/// <summary>Ledger Paper (11x17 in)</summary>
			Ledger,
			/// <summary>Legal Paper (8.4x14 in)</summary>
			Legal,
			/// <summary>Letter Paper (8.5x11 in)</summary>
			Letter,
			/// <summary>Letter-Government Paper (8x10.5 in)</summary>
			LetterGovernment,					
			/// <summary>Quarto Paper (8x10 in)</summary>
			Quarto,
			/// <summary>Statement Paper (5.5x8.5in)</summary>		
			Statement			
		};		
		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the size of the particular paper type.
		/// </summary>
		/// <param name="paperType">PaperType: <see cref="Com.Delta.PrintManager.Engine.Paper.Type">Type</see> of paper.</param>
		public static Size GetPaperSize(Paper.Type PaperType)
		{
			if (types.Contains(PaperType))
				return (Size)types[PaperType];
			else
				return (Size)types[Type.A4];
		}

		/// <summary>
		/// Returns Paper.Type based on string
		/// </summary>
		/// <param name="PaperType">PaperType: string name of a paper type</param>
		/// <returns>Returns <see cref="Com.Delta.PrintManager.Engine.Paper.Type">Type</see> of paper</returns>
		/// <remarks>If the string is not a valid paper type, then A4 is passed back as the default
		/// paper type</remarks>
		internal static Type GetType(string PaperType)
		{
			switch (PaperType)
			{
				case "A0" : return Type.A0;
				case "A1" : return Type.A1;
				case "A2" : return Type.A2;
				case "A3" : return Type.A3;
				case "A4" : return Type.A4;
				case "A5" : return Type.A5;
				case "A6" : return Type.A6;
				case "B0" : return Type.B0;
				case "B1" : return Type.B1;
				case "B2" : return Type.B2;
				case "B3" : return Type.B3;
				case "B4" : return Type.B4;
				case "B5" : return Type.B5;
				case "B6" : return Type.B6;
				case "Letter" : return Type.Letter;
				case "LetterGovernment" : return Type.LetterGovernment;
				case "Legal" : return Type.Legal;
				case "Ledger" : return Type.Ledger;
				case "Quarto" : return Type.Quarto;
				case "Executive" : return Type.Executive;
				case "EnvelopeMonarch" : return Type.EnvelopeMonarch;
				case "EnvelopeC5" : return Type.EnvelopeC5;
				case "EnvelopeDL" : return Type.EnvelopeDL;
				case "Foolscap" : return Type.Foolscap;
				case "Statement" : return Type.Statement;
				case "Custom" : return Type.Custom;
				default : return Type.A4;
			}
			
		}


		#endregion
	}
}
