using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class Arduino : MonoBehaviour
{
    SerialPort serialPort = new SerialPort("COM3", 9600);

    void Start()
    {
        serialPort.Open();
        serialPort.ReadTimeout = 500;

    }

    public void rojoOn()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Write("A");
        }
    }

    public void verdeOn()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Write("B");
        }
    }

    public void amarilloOn()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Write("C");
        }
    }

    public void blancoOn()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Write("D");
        }
    }

    public void vibrarOn()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Write("E");
        }
    }

    public float leerX()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Write("F");
            return float.Parse(serialPort.ReadLine());
        }
        else
            return 0;
    }

    public float leerY()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Write("G");
            return float.Parse(serialPort.ReadLine());
        }
        else
            return 0;
    }



    public void cerrarPort()
    {
        serialPort.Close();
    }
}
