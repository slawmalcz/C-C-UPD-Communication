using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestV3
{
    class FileProcesor
    {
        private Dictionary<int, byte[]> createFile = new Dictionary<int, byte[]>();
        private string filePath;
        public int numOfPacket;

        public Dictionary<int, byte[]> CreateFile { get => createFile; set => createFile = value; }
        public string FilePath { get => filePath; set => filePath = value; }

        public FileProcesor(string filePath)
        {
            this.FilePath = filePath;
            createFile = GetPackages();
        }

        public FileProcesor(string nameAndExtension, int numOfPackage)
        {
            this.FilePath = nameAndExtension;
            this.numOfPacket = numOfPackage;
        }

        public FileProcesor()
        {
        }

        public Dictionary<int,byte[]> GetPackages(int byteBuffer = 1024)
        {
            var byteArray = File.ReadAllBytes(this.FilePath);
            var package = splitArray(byteArray, byteBuffer);
            return package;
        }

        public string getNameAndExtension()
        {
            return Path.GetFileName(this.FilePath);
        }

        private Dictionary<int,byte[]> splitArray(byte[] array, int size)
        {
            Dictionary<int, byte[]> ret = new Dictionary<int, byte[]>();
            int currentIteration = 0;
            while (0 < array.Length)
            {
                ret.Add(currentIteration,array.Take(size).ToArray());
                array = array.Skip(size).ToArray();
                currentIteration++;
            }
            this.numOfPacket = currentIteration;
            return ret;
        }

        public void insertPackage(int index, byte[] byteArray)
        {
            if (!this.CreateFile.ContainsKey(index))
            {
                this.CreateFile.Add(index, byteArray);
            }
        }

        private bool checkFinalSum(int numOfPackages)
        {
            for(int i = 0; i < numOfPackages; i++)
            {
                if (!this.CreateFile.ContainsKey(i)) return false;
            }
            return true;
        }

        public void combineFile(int numOfPackages)
        {
            if (checkFinalSum(numOfPackages))
            {
                List<byte> fileData = new List<byte>();
                for (int i = 0; i < numOfPackages; i++)
                {
                    fileData.AddRange(this.CreateFile[i].ToList());
                }
                // Null end cutting 
                for (int i=1; i<= numOfPackages; i++)
                {
                    if (fileData[numOfPackages - i] == 0)
                    {
                        fileData.RemoveAt(numOfPackages - i);
                    }
                    else
                    {
                        break;
                    }
                }
                byte[] finalData = fileData.ToArray();

                File.WriteAllBytes("Copy-"+ filePath, finalData);
            }
            else
            {
                throw new Exception("Wrong num of packages");
            }
        }
    }
}
