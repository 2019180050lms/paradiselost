using DummyClient;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
	ServerSession _session = new ServerSession();
	public void Send(ArraySegment<byte> sendBuff)
    {
		_session.Send(sendBuff);
    }

    void Start()
    {
		// DNS (Domain Name System)
		string host = Dns.GetHostName();
		IPHostEntry ipHost = Dns.GetHostEntry(host);
		//IPAddress ipAddr = IPAddress.Parse("192.168.219.100");
		IPAddress ipAddr = IPAddress.Parse("27.119.175.22");
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

		Connector connector = new Connector();

		connector.Connect(endPoint,
			() => { return _session; },
			1);
		DontDestroyOnLoad(this);
	}

    void Update()
    {
		List<IPacket> list = PacketQueue.Instance.PopAll();
		foreach (IPacket packet in list)
			PacketManager.Instance.HandlePacket(_session, packet);
    }
}
