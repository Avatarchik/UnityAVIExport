﻿using UnityEngine;
using System.IO;
using UnityEngine.Rendering;
using GitHub.secile.Avi;

namespace com.relativedistance.UnityAVIExport
{	
	public class AVIExport
	{
		Camera cam;
		RenderTexture rt;
		MjpegWriter writer;
		Texture2D tex;
		int quality;
		int frameCounter,gFrameRate;
		float fps;
		bool currentlyRecording = false;
		
		public void Init(Camera c, int w, int h, float aviFrameRate, int gameFrameRate, int jpgQuality)
		{
			if (writer!=null)
			{
				writer.Close();
				writer =null;
			}
			
			MemoryStream m = new MemoryStream();
			gFrameRate = gameFrameRate;
			fps = aviFrameRate;
			
			if (c == null) 
			{ 
				cam = Camera.main; 
			}
			else 
			{ 
				cam = c; 
			}
			
			rt = new RenderTexture(w,h,24);
			tex = new Texture2D(rt.width, rt.height);
			quality = jpgQuality;	
			
			writer = new MjpegWriter( m, rt.width, rt.height, fps);
		}
		
		public void startRecording()
		{
			currentlyRecording = true;
		}
		
		public void DoUpdate()
		{
			if (currentlyRecording) 
			{ 
				frameCounter++;
				if (frameCounter % (int)(gFrameRate/fps)== 0) 
				{
					recordFrame(); 
				}
			}		
		}
		
		void recordFrame()
		{
			Camera.main.targetTexture = rt;
			RenderTexture.active = rt;
			Camera.main.Render();
			tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
			tex.Apply();
			
			var bytes = tex.EncodeToJPG(quality);		
			writer.AddImage(bytes);
			
			RenderTexture.active = null;
			Camera.main.targetTexture = null;
		}
		
		public void stopRecording()
		{
			currentlyRecording = false;		
		}
		
		public byte[] getAVIByteArray()
		{
			byte[] b = writer.Close();
			writer =null;
			return b;
		}
		
	}
}