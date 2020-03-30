﻿using Studyzy.IMEWLConverter.IME;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ToolGood.PinYin.Pretreatment
{
    class Program
    {
        static void Main(string[] args)
        {
            // 预处理
            // 第一步 处理搜狗词库
            if (File.Exists("scel_1.txt") == false) {
                var scel_1 = GetWords();
                File.WriteAllText("scel_1.txt", string.Join("\n", scel_1));
                scel_1.Clear();
            }
            // 第二步 精简词库  

            // 第三步 获取词的所有拼音

            // 第四步 获取网上的拼音
            if (File.Exists("pinyin_1.txt") == false) {
                var pinyin_1 = GetPinYin();
                File.WriteAllText("pinyin_1.txt", string.Join("\n", pinyin_1));
                pinyin_1.Clear();
            }

            // 第五步 分离 单字拼音 和 词组拼音
            if (File.Exists("pinyin_2_one.txt") == false) {
                var txt = File.ReadAllText("pinyin_1.txt");
                var lines = txt.Split('\n');
                List<string> ones = new List<string>();
                List<string> mores = new List<string>();
                foreach (var line in lines) {
                    var sp = line.Split(',');
                    if (GetLength(sp[0])==1) {
                        ones.Add(line);
                    } else {
                        mores.Add(line);
                    }
                }
                File.WriteAllText("pinyin_2_one.txt", string.Join("\n", ones));
                File.WriteAllText("pinyin_2_more.txt", string.Join("\n", mores));
                ones.Clear();
                mores.Clear();
            }
            // 第六步 合并 单字拼音
            if (File.Exists("pinyin_3_one.txt") == false) {

            }
            // 第七步 检查 拼音数 与 词组长度不一样的
            if (File.Exists("pinyin_4_ok.txt") == false) {
                var txt = File.ReadAllText("pinyin_2_more.txt");
                var lines = txt.Split('\n');
                List<string> oks = new List<string>();
                List<string> errors = new List<string>();
                foreach (var line in lines) {
                    var sp = line.Split(',');
                    if (GetLength(sp[0]) == sp.Length-1) {
                        oks.Add(line);
                    } else {
                        errors.Add(line);
                    }
                }
                File.WriteAllText("pinyin_4_ok.txt", string.Join("\n", oks));
                File.WriteAllText("pinyin_4_error.txt", string.Join("\n", errors));
            }




        }





        static List<string> GetWords()
        {
            var files = Directory.GetFiles("Scel");
            HashSet<string> list = new HashSet<string>();

            foreach (var file in files) {
                GetWords(file, list);
            }
            return list.OrderBy(q=>q).ToList();
        }

        static void GetWords(string file, HashSet<string> list)
        {
            SougouPinyinScel scel = new SougouPinyinScel();
            var t = scel.Import(file);

            foreach (var item in t) {
                var w = item.Word.Trim();
                var py = string.Join(",", item.PinYin);

                list.Add($"{w} {py}");
            }
        }

        static List<string> GetPinYin()
        {
            var files = Directory.GetFiles("dict", "*.txt");
            HashSet<string> list = new HashSet<string>();

            foreach (var file in files) {
                GetPinYin(file, list);
            }
            return list.OrderBy(q => q).ToList();
        }

        static void GetPinYin(string file, HashSet<string> list)
        {
            var text = File.ReadAllText(file);
            var line = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var l in line) {
                if (string.IsNullOrWhiteSpace(l)) {
                    continue;
                }
                var sp = l.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                list.Add(string.Join(",", sp));
            }
        }


        private static int GetLength(string text)
        {
            var stringInfo = new System.Globalization.StringInfo(text);
            return stringInfo.LengthInTextElements;
        }

        public static String Substring(String text, int start, int end)
        {
            var stringInfo = new System.Globalization.StringInfo(text);
            return stringInfo.SubstringByTextElements(start, end);
        }

    }
}