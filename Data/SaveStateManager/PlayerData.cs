using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System;

namespace UPDB.Data.SaveStateManager
{
    ///<summary>
    /// Manage save and load with cryptable keys
    ///</summary>
    [CreateAssetMenu(fileName = "NewPlayerData", menuName = "UPDB/Data/SaveState Manager/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        [SerializeField]
        private string _username = string.Empty;

        [SerializeField]
        private int _score = 0;

        [SerializeField]
        private int _levelState = 0;

        [SerializeField]
        private bool _isDead = false;

        [SerializeField]
        private Vector3 _playerPosition = Vector3.zero;

        [Header("NEW SAVE"), SerializeField]
        private string _saveName = string.Empty;

        /// <summary>
        /// last saved key, currently also used to load key
        /// </summary>
        private string _usedKey = string.Empty;

        private Key _playerKey;
        private static string SaveFilePath = string.Empty;

        #region Public API

        private Dictionary<string, string> _typeBlackBoard = new Dictionary<string, string>()
        {
            {"int", "0" },
            {"float", "1" },
            {"bool", "2" },
            {"string", "3" }
        };

        public Vector3 PlayerPosition => _playerPosition;

        public string UsedKey
        {
            get
            {
                return _usedKey;
            }

            set
            {
                _usedKey = value;
            }
        }

        #endregion

        /// <summary>
        /// save the current state
        /// </summary>
        public void Save()
        {
            SaveFilePath = Path.Combine(Application.persistentDataPath, $"{_saveName}.json");
            _usedKey = Crypt();
            _playerKey = new Key(_usedKey);
            string json = JsonUtility.ToJson(_playerKey, true);
            File.WriteAllText(SaveFilePath, json);
            Debug.Log("File saved at :" + SaveFilePath);
        }

        /// <summary>
        /// load the saved state
        /// </summary>
        public void Load()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorUtility.DisplayDialog("load last save ? ", "any non saved work will be erased !", "Load"))
                return;
#endif
            SaveFilePath = Path.Combine(Application.persistentDataPath, $"{_saveName}.json");
            string jsonToLoad = File.ReadAllText(SaveFilePath);
            JsonUtility.FromJsonOverwrite(jsonToLoad, _playerKey);
            _usedKey = _playerKey.key;
            Decrypt();
        }

        /// <summary>
        /// create a key from every given variables(you have to set manually variables you want to be crypted) PS : three manual set up : baseKey, list of convertions, and get decimal index.
        /// </summary>
        /// <returns>Key</returns>
        public string Crypt()
        {
            //define longest number of char needed to define length of a variable( ex : 234 is length of 3, so, he needs only 1 char) and every variables will have the same number of char in their length definition.
            int longestCryptageRead = 1;
            //key to return
            string key = string.Empty;
            //skeleton, set at the very beginning of the key, with "_", define the number of char to read from a length definition(if longest cryptage read is 3, then skeleton will be "___")
            string skeleton = string.Empty;
            //baseKey define, in order, wich type the variables are, according to the Type blackboard above(manual set up needed !)
            string baseKey = _typeBlackBoard["string"] + _typeBlackBoard["int"] + _typeBlackBoard["int"] + _typeBlackBoard["bool"] + _typeBlackBoard["float"] + _typeBlackBoard["float"] + _typeBlackBoard["float"];
            //manual set up needed !, define all the variables that will be put in the variable list and converted to a key.
            string[] _cryptedVarList =
            {
                VarToString(_username),
                VarToString(_score),
                VarToString(_levelState),
                VarToString(_isDead),
                VarToString(_playerPosition.x),
                VarToString(_playerPosition.y),
                VarToString(_playerPosition.z)
            };

            //initialise skeleton from longest cryptage read
            for (int i = 0; i < longestCryptageRead; i++)
                skeleton += "_";

            //initialise length of baseKey
            string baseKeyLength = baseKey.Length.ToString();

            //normalize length of baseKeyLength, if skeleton is 2, but basekeyLength is "1", then baseKeyLength will become "01"
            while (baseKeyLength.Length < longestCryptageRead)
                baseKeyLength = "0" + baseKeyLength;

            //manual set up needed !, take every decimal number crypted in the list, and add the decimal number
            _cryptedVarList[4] += GetDecimal(_playerPosition.x);
            _cryptedVarList[5] += GetDecimal(_playerPosition.y);
            _cryptedVarList[6] += GetDecimal(_playerPosition.z);

            //for each variable in the crypted list, add the read length before it(according to skeleton, if skeleton is 2, and variable is "345", then it will become "03345")
            for (int i = 0; i < _cryptedVarList.Length; i++)
                _cryptedVarList[i] = GetReadLength(_cryptedVarList[i]) + _cryptedVarList[i];

            //add each variable of the crypted list in the final key, according to the crypted list order
            foreach (string variable in _cryptedVarList)
                key += variable;

            //add the skeleton, the base key and his length, to the final key
            key = skeleton + baseKeyLength + baseKey + key;

            //return the key
            return key;

            #region Cryptage Functions

            //convert any variables in number chain(to string type) according to initial variable type, return convertedVar, take the variable in argument
            string VarToString<T>(T variable)
            {
                //variable to return
                string returnVar = string.Empty;

                //make different program depending of given variable's type
                if (variable is string)
                {
                    //make an array of byte representing ASCII code of given string
                    byte[] asciiVar = Encoding.ASCII.GetBytes(variable.ToString());

                    //for each ASCII byte of array, size to 3 char the length of it, so that every ASCII is 3 length(ex : 127 will stay 127, but 76 will become 076)
                    foreach (byte character in asciiVar)
                    {
                        string characterSized = character.ToString();

                        while (characterSized.Length < 3)
                        {
                            characterSized = "0" + characterSized;
                        }
                        //variable to return is equal to the chain of every Byte ASCII code sized and converted to a string.
                        returnVar += characterSized;
                    }
                }
                else if (variable is int)
                {
                    //return the int converted to a string
                    returnVar = variable.ToString();
                }
                else if(variable is bool)
                {
                    //if boolean is true, return 1, if not, return 0
                    if (variable.ToString().ToLower() == "true")
                        returnVar = "1";
                    else if (variable.ToString().ToLower() == "false")
                        returnVar = "0";
                }
                else if (variable is float)
                {
                    //start like int, by convert it to a string.
                    returnVar = variable.ToString();
                    //the index of decimal, 0 means no decimal, 1 means a decimal to the right of the first character at the left.
                    int decimalIndex = 0;

                    //pass on every character of the variable, and delete the decimal, while registering the index of it at the same time
                    for (int i = 0; i < returnVar.Length; i++)
                    {
                        if (returnVar[i] == '.' || returnVar[i] == ',')
                        {
                            decimalIndex = i;
                            returnVar = returnVar.Remove(i, 1);
                        }
                    }

                    //if decimal index needs more character to define than every other variables, then longest cryptage read and skeleton will become this length.
                    if (decimalIndex.ToString().Length > longestCryptageRead)
                        longestCryptageRead = decimalIndex.ToString().Length;
                }
                //like decimals, if variable length needs more character to define than every other variables, then longest cryptage read and skeleton will become this length.
                if (returnVar.Length.ToString().Length > longestCryptageRead)
                    longestCryptageRead = returnVar.Length.ToString().Length;

                //return the variable
                return returnVar;
            }

            //get read length of a variable, according to the current longest cryptage read
            string GetReadLength(string variable)
            {
                //initialize the readLength to the length of variable
                string readLength = variable.Length.ToString();

                //normalize length of ReadLength, according to longest crypted read
                while (readLength.Length < longestCryptageRead)
                {
                    readLength = "0" + readLength;
                }

                //return length
                return readLength;
            }

            //get decimal index of a float variable, according to the current longest cryptage read
            string GetDecimal(float variable)
            {
                //initialize decimal index to "null"(0)
                string decimalIndex = "0";
                //initialize variable to read
                string varToSet = variable.ToString();

                //pass on each char of variable, and register decimal at index of detected decimal
                for (int i = 0; i < varToSet.Length; i++)
                    if (varToSet[i] == '.' || varToSet[i] == ',')
                        decimalIndex = i.ToString();

                //normalize length of decimal, according to longest cryptage read
                while (decimalIndex.Length < longestCryptageRead)
                {
                    decimalIndex = "0" + decimalIndex;
                }

                //return decimal index
                return decimalIndex;
            } 

            #endregion
        }

        /// <summary>
        /// take the current saved key, for the moment, use the variable "latestKey" and Decrypt every variables in it(you have to set up manually sets of variables ! (at the end of decrypt function))
        /// </summary>
        public void Decrypt()
        {
            //variable that define the number of char in length variables, according to skeleton equivalent of the "LongestCryptageRead" of Crypt function)
            int readLengthStandard = 0;
            //variable that contain baseKey
            string baseKey = string.Empty;
            //variable that contains key
            string key = _usedKey;
            //real time index of program reading
            int index = 0;
            //list of every founded variables
            List<string> variableList = new List<string>();
            //list of every decrypted variables, to generic types
            List<object> decryptedVarList = new List<object>();

            //set up readLengthStandard, according to skeleton
            while (key[index] == '_')
            {
                readLengthStandard++;
                index++;
            }

            //find the baseKey
            FindBaseKey();

            //for each variables(number given by the baseKey))
            for (int i = 0; i < baseKey.Length; i++)
                FindVariable();

            //for each variable of the list, convert to object type, depending of baseKey type
            for (int i = 0; i < baseKey.Length; i++)
                StringToVar(variableList[i], Convert.ToInt32(baseKey[i].ToString()));


            //Manual set up needed !, assign variables to the correct list object, according to key order
            _username = (string)decryptedVarList[0];
            _score = (int)decryptedVarList[1];
            _levelState = (int)decryptedVarList[2];
            _isDead = (bool)decryptedVarList[3];
            _playerPosition = new Vector3((float)decryptedVarList[4], (float)decryptedVarList[5], (float)decryptedVarList[6]);


            #region Decryptage Functions

            //when called, find the baseKey, depending of readLength standard
            void FindBaseKey()
            {
                //length of value to read
                string readNumber = string.Empty;

                //pass on every char of baseKey length, then put them in readNumber, to make an int of it later, then update index value.
                for (int i = index; i < index + readLengthStandard; i++)
                    readNumber += key[i];
                index += readLengthStandard;

                //convert value length to int
                int valueLength = Convert.ToInt32(readNumber);

                //pass on every char of baseKey, and put them in baseKey variable, then update index value.
                for (int i = index; i < index + valueLength; i++)
                    baseKey += key[i];
                index += valueLength;
            }

            //find a variable in the key, starting at index value, and length of reading depending of read length standard
            void FindVariable()
            {
                //value of reading length
                string readNumber = string.Empty;
                //string containing variable
                string foundedVariable = string.Empty;

                //pass on every char of value length, then put them in readNumber, to make an int of it later, then update index value.
                for (int i = index; i < index + readLengthStandard; i++)
                    readNumber += key[i];
                index += readLengthStandard;

                //length of variable converted to int
                int valueLength = Convert.ToInt32(readNumber);

                //pass on every char of variable, and put them in founded variable, then update index value.
                for (int i = index; i < index + valueLength; i++)
                    foundedVariable += key[i];
                index += valueLength;

                //add the founded variable int the variable list
                variableList.Add(foundedVariable);
            }

            //convert string variables of the variable list into original variables, depending of baseKey type values, and type blackboard, converted variable will be added in the converted variable list
            void StringToVar(string variable, int type)
            {
                //variable to set, object type
                object returnVar = null;

                //make diferent operations, depending of variable type(according to type blackboard)
                if (type == Convert.ToInt32(_typeBlackBoard["int"]))
                {
                    //convert variable into int
                    returnVar = Convert.ToInt32(variable);
                }
                else if (type == Convert.ToInt32(_typeBlackBoard["float"]))
                {
                    //variable containing decimal index
                    string decimalIndex = string.Empty;

                    //set up decimal index with last values of variable(depending of read length standard)
                    for (int i = variable.Length - readLengthStandard; i < variable.Length; i++)
                        decimalIndex += variable[i].ToString();

                    //set up return var to variable(removing the decimal index part)
                    for (int i = 0; i < variable.Length - readLengthStandard; i++)
                        returnVar += variable[i].ToString();

                    //decimal index converted to int
                    int decimalIndexInt = Convert.ToInt32(decimalIndex);
                    //variable converted to float
                    float returnVarFloat = Convert.ToInt32(returnVar);

                    //if decimal index is 0, then do not add any decimal, and convert variable to float, else, divide the value by 10^ numberLength - decimal index, to put decimal at the right index
                    if (decimalIndexInt != 0)
                        returnVar = returnVarFloat / (float)(Mathf.Pow(10, (returnVar.ToString().Length - decimalIndexInt)));
                    else
                        returnVar = returnVarFloat;
                }
                else if (type == Convert.ToInt32(_typeBlackBoard["bool"]))
                {
                    //put the return var at 0 or 1, depending of boolean value
                    if (variable == "0")
                        returnVar = false;
                    else if (variable == "1")
                        returnVar = true;
                }
                else if (type == Convert.ToInt32(_typeBlackBoard["string"]))
                {
                    //make a list of bytes that will contain every ascii char
                    List<byte> charASCIIList = new List<byte>();

                    //divide variable to sets of 3 char, representing every ASCII characters, then add them to byte list
                    for (int i = 0; i < variable.Length; i += 3)
                        charASCIIList.Add(Convert.ToByte((variable[i].ToString() + variable[i + 1].ToString() + variable[i + 2].ToString())));

                    //convert ASCII code of byte array, to string characters 
                    returnVar = Encoding.ASCII.GetString(charASCIIList.ToArray());
                }

                //add return variable to decrypted variables list
                decryptedVarList.Add(returnVar);
            } 

            #endregion
        }
    }
}