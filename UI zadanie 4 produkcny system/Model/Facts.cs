using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace UI_zadanie_4_produkcny_system.Model
{
    public class Facts
    {
        private string _factString;

        public Facts(string _fstring)
        {
            this._factString = _fstring;
        }

        public string getinfo()
        {
            return this._factString;
        }

       
        public static void LoadFacts(string path, MainWindow window)
        {
            window.listBox.Items.Clear();
            using (StreamReader reader = File.OpenText(path))
            {
                string Row = " ";
                while ((Row = reader.ReadLine()) != null)
                {
                    window.getCurrentMainWindow().listBox.Items.Add(Row);
                    Logic.factsList.Add(new Facts(Row));
                }

            }


        }

      
        public static Boolean exist(string tocheck)
        {
            foreach (Facts targert in Logic.factsList)
            {
                if (targert.getinfo().SequenceEqual(tocheck))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<string> get_unknowns(string redundant_but_required_cause_static)
        {
            List<string> resutltList = new List<string>();
            string[] resultStrings = redundant_but_required_cause_static.Split(' ');
            Regex regex = new Regex(@"^[A-Z]{1}[a-z]");
            foreach (string resultString in resultStrings)
            {
                if (regex.IsMatch(resultString))
                {
                    resutltList.Add(resultString);
                }
            }
            return resutltList;
        }

        public static Boolean issexy(string fact)
        {
            foreach (Facts VARIABLE in Logic.factsList)
            {
                if (VARIABLE.getinfo().SequenceEqual(fact))
                {
                    return true;
                }
            }
            return false;
        }

        public static int factPosition(string fact)
        {
            for (int i = 0; i < Logic.factsList.Count; i++)
            {
                if (Logic.factsList[i].getinfo().SequenceEqual(fact))
                    return i;
            }
            return 0;
        }

        public static void add(MainWindow window)
        {
            window.listBox.Items.Clear();
            foreach (Facts facts in Logic.factsList)
            {
                window.listBox.Items.Add(facts.getinfo());
            }
            
        }
    }
}