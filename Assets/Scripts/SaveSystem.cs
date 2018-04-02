﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour {

    static List<GameObject> objectForSave = new List<GameObject>();

    public static InputField saveName;
    public static Text loadName;

    private static string savePath;
    void Start ()
    {
        savePath = Application.dataPath + "/saves/";
    }

    // Update is called once per frame
    void Update ()
    {
        changeLoadName();

        if (Input.GetKeyDown(KeyCode.F5))
        {            
            Save();
        }

        if(Input.GetKeyDown(KeyCode.F9))
        {
            Load();
        }
	}

    public static void Save()
    {
        //и стенки и юниты, всё это объекты...
        objectForSave = new List<GameObject>();
        objectForSave.AddRange(GameObject.FindGameObjectsWithTag("Walls"));
        objectForSave.AddRange(GameObject.FindGameObjectsWithTag("Units"));



        // с е р и а л и з а ц и я объектов
        int num = 0;
        ObjectData[] data = new ObjectData[objectForSave.Capacity];
        foreach (GameObject element in objectForSave)
        {
            data[num] = new ObjectData(element);           

            ++num;
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(savePath + saveName.text + ".glaz", FileMode.Create);

        bf.Serialize(stream, data);
        stream.Close();
    }


    private static void Load()
    {
        if (File.Exists(savePath + loadName.text))
        {
            //Очищаем сцену перед загрузкой сохранения
            List<GameObject> objectToDelete = new List<GameObject>();
            objectToDelete.AddRange(GameObject.FindGameObjectsWithTag("Walls"));
            objectToDelete.AddRange(GameObject.FindGameObjectsWithTag("Units"));

            foreach (GameObject o in objectToDelete)
            {
                Destroy(o);
            }

            //Загрузка
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(savePath + loadName.text, FileMode.Open);

            ObjectData[] data = bf.Deserialize(stream) as ObjectData[];
            stream.Close();
            foreach (ObjectData element in data)
            {
                Debug.Log("Loading...");
                SpawnOnLoad(element);
            }
            
        }
    }

    private static void SpawnOnLoad(ObjectData loadedObject)
    {
        GameObject objectToSpawn;

        objectToSpawn = GameObject.Find(loadedObject.type);
        GameObject newObject 
            = Instantiate(objectToSpawn, 
                    new Vector3(loadedObject.coordinates[0], loadedObject.coordinates[1], 1), Quaternion.identity);
        newObject.tag = loadedObject.tag;

        Debug.Log("LOADED");
    }

    public static void SaveButton(string savePath)
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

    int position = 0;
    public void LoadButton()
    {
        //if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Load();
            Debug.Log("CLICK");
        }
    }
    public void PositionPrev()
    {
        --position;
        Debug.Log(position);
    }
    public void PositionNext()
    {
        ++position;
        Debug.Log(position);
    }
    public void changeLoadName()
    {
        List<string> loadFiles = new List<string>();
        DirectoryInfo di = new DirectoryInfo(savePath);

        foreach (FileInfo file in di.GetFiles())
        {
            loadFiles.Add(file.Name);
        }

        if(position >= loadFiles.Count)
        {
            position = 0;
            Debug.Log("Changed to " + position);
        }
        if (position < 0)
        {
            position = loadFiles.Count - 1;
            Debug.Log("Changed to " + position);
        }

        loadName.text = loadFiles[position];
    }

}

[Serializable]
public class ObjectData
{
    public float[] coordinates;
    public string type;
    public string tag;

    public ObjectData(GameObject obj)
    {
        type = obj.name.Remove(obj.name.Length - 7); // нужно удалить окончание "(Clone)" это 7 символов.
        tag = obj.tag;

        coordinates = new float[2];
        coordinates[0] = obj.transform.position.x;
        coordinates[1] = obj.transform.position.y;
    }   
}
