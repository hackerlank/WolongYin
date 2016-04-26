using Mono.Xml;
using System.Collections.Generic;

public class GameSetupXmlClass : Singleton<GameSetupXmlClass>
{
#region structs

	public struct BattleStruct
	{
		public int max_turn;
		public float one_star_power_val;
	}

#endregion

	public string FileName = "game_setup.xml";


	private BattleStruct m_Battle;


	public BattleStruct battle { get { return m_Battle;} }


	public void Parse(string text)
	{
		SecurityParser sp = new SecurityParser();
		sp.LoadXml(text);
		System.Security.SecurityElement se = sp.ToXml();
		if (se.Children != null)
		{
			foreach (System.Security.SecurityElement ce in se.Children)
			{
				Parse(ce);
			}
		}

	}
	public void Parse(System.Security.SecurityElement se)
	{
		if (se.Tag.ToLower() == "battle")
		{
			m_Battle = new BattleStruct();
			m_Battle.max_turn = int.Parse(se.Attribute("max_turn"));
			m_Battle.one_star_power_val = float.Parse(se.Attribute("one_star_power_val"));
		}

	}
	public bool Load(string folder)
	{
		try
		{
			string sText = XmlUtility.LoadFile(folder, FileName);
			if (sText != null)
			{
				Parse(sText);
				return true;
			}
			else return false;
		}catch(System.Exception ex)
		{
			Logger.instance.Error(FileName + ": " + ex.Message);
			return false;
		}

	}
}
