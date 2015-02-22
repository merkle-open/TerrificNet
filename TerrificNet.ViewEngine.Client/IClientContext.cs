﻿namespace TerrificNet.ViewEngine.Client
{
	public interface IClientContext
	{
		void WriteLiteral(string content);
		void WriteExpression(IClientModel model);
		IClientModel BeginIterate(IClientModel model);
		void EndIterate();
		void BeginIf(IClientModel model);
		void EndIf();
		void ElseIf();
	}
}