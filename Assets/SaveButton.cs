﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class SaveButton : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        SaveSystem.Save();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void Go(string savePath)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            //Создаём папку сохранений если её ещё нет
            if (File.Exists(savePath) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            }
            //Если имея сохранения не введено, то оно стадартное  
            if (saveName.text == "")
            {
                int files_amount = 1;
                DirectoryInfo di = new DirectoryInfo(savePath);
                foreach (var fi in di.GetFiles())
                {
                    ++files_amount;
                }
                saveName.text = "Save" + files_amount;
            }

            Save();
        }
    }
}