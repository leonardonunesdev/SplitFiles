using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SplitFiles
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //List<byte[]> arquivos = new List<byte[]>();
            //string fileName = @"C:\Users\leonardo.santos\Desktop\Zip Files\Testes.zip";
            //int blockSize = 15000000;
            //FileStream fsIn = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            //string basePath = System.IO.Path.GetDirectoryName(fileName);
            //int fileSize = (int)fsIn.Length;
            //int m_segmentSize = 15000000;
            //int segments = fileSize / m_segmentSize;
            //int remainder = fileSize % m_segmentSize;
            //byte[] buffer = new byte[blockSize];// 15 MB

            //if (remainder > 0) segments++;

            //for (int i = 0; i < segments; i++)
            //{
            //    FileStream fsOut = new FileStream(fileName.Replace(".zip","") + i + ".zip", FileMode.Create, FileAccess.Write);
            //    int blocks = (int)(m_segmentSize / blockSize);

            //    if (remainder > 0 && i < segments - 1)
            //    {
            //        for (int j = 0; j < blocks; j++)
            //        {
            //            //probably should just check the int ret value then write those blocks
            //            // then if the ret value < 1024, just write the remainder & exit
            //            fsIn.Read(buffer, 0, blockSize);
            //            fsOut.Write(buffer, 0, blockSize);
            //        }
            //    }
            //    else
            //    {
            //        int finalBlocks = (int)(remainder / blockSize);
            //        int lastBlock = (int)(remainder % blockSize);
            //        for (int k = 0; k < finalBlocks; k++)
            //        {
            //            fsIn.Read(buffer, 0, blockSize);
            //            fsOut.Write(buffer, 0, blockSize);
            //        }
            //        fsIn.Read(buffer, 0, lastBlock);
            //        fsOut.Write(buffer, 0, lastBlock);
            //    }
            //    fsOut.Flush();
            //    fsOut.Close();
            //}
            //fsIn.Close();

            //Other documents
            string folderPath = @"C:\Users\leonardo.santos\Desktop\Testes";
            string[] filesInsideFolder = Directory.GetFiles(folderPath);

            //PDF
            string[] fileFolderPDF = Directory.GetFiles(@"C:\Users\leonardo.santos\Desktop\PDF");
            FileInfo filePDF = new FileInfo(fileFolderPDF[0]);
            int fileSizePDF = (int)filePDF.Length;

            List<string> fileZipTogheter = new List<string>();
            int fileMaxSize = 15000000;// 15MB
            int segmentSize = fileSizePDF;// Controla a soma do tamanho dos arquivos na pasta

            List<byte[]> zipFiles = new List<byte[]>();
            bool pdfFileAdded = false;

            for (int numFile = 0; numFile < filesInsideFolder.Length; numFile++)
            {
                FileInfo file = new FileInfo(filesInsideFolder[numFile]);
                int fileSize = (int)file.Length;

                segmentSize += fileSize;

                if (segmentSize > fileMaxSize || numFile == (filesInsideFolder.Length - 1))
                {
                    using (var compressedFileStream = new MemoryStream())
                    {
                        using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update, false))
                        {
                            for (int numFileToZip = 0; numFileToZip < fileZipTogheter.Count; numFileToZip++)
                            {
                                FileInfo fileToZip = new FileInfo(fileZipTogheter[numFileToZip]);
                                ZipArchiveEntry zipEntry = zipArchive.CreateEntry(fileToZip.Name, CompressionLevel.Optimal);

                                using (var zipEntryStream = zipEntry.Open())
                                {
                                    //Adiciona todos os bytes em um arquivo
                                    fileToZip.OpenRead().CopyTo(zipEntryStream);
                                }
                            }

                            if (!pdfFileAdded)
                            {
                                ZipArchiveEntry zipEntry = zipArchive.CreateEntry(filePDF.Name, CompressionLevel.Optimal);
                                using (var zipEntryStream = zipEntry.Open())
                                {
                                    zipEntryStream.Write(new byte[filePDF.Length], 0, (int)filePDF.Length);
                                    pdfFileAdded = true;
                                }
                            }
                        }

                        segmentSize = fileSize;// Reseta o contador para que o primeiro aquivo seja o último arquivo lido
                        fileZipTogheter = new List<string>();// Reseta a lista com os arquivos a serem zipados
                        fileZipTogheter.Add(filesInsideFolder[numFile]);// Começa a nova lista com o último arquivo lido
                        zipFiles.Add(compressedFileStream.ToArray());

                    }
                }
                else
                {
                    fileZipTogheter.Add(filesInsideFolder[numFile]);
                }
            }

            for(int numZip = 0; numZip < zipFiles.Count; numZip++)
            {
                File.WriteAllBytes(@"C:\Users\leonardo.santos\Desktop\Zip Files\zipTeste" + numZip + ".zip", zipFiles[numZip]); //Teste o zip
            }
        }
    }
}
