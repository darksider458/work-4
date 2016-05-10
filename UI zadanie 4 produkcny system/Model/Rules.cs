using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UI_zadanie_4_produkcny_system.Model
{
    public class Rules
    {
        protected string name;
        protected List<string> conditionList = new List<string>();
        protected List<Regex> regexList = new List<Regex>();
        protected List<string> actionList = new List<string>();
        protected List<List<Dictionary<string, string>>> VariablesList = new List<List<Dictionary<string, string>>>();
        protected List<Dictionary<string, string>> finalList = new List<Dictionary<string, string>>();
        protected Boolean special;

        public Rules(string name, List<string> conditionList, List<string> actionList)
        {
            this.name = name;
            this.conditionList = conditionList;
            this.actionList = actionList;

        }

        public static void Load_za_Rules(string path, MainWindow window)
        {
            window.listBox1.Items.Clear();
            using (StreamReader reader = File.OpenText(path))
            {

                string Row = " ";
                while ((Row = reader.ReadLine()) != null)
                {
                    string namestring = Row;
                    Row = reader.ReadLine();
                    string requirementsstring = Row;
                    Row = reader.ReadLine();
                    string actionsstring = Row;
                    window.listBox1.Items.Add(namestring);
                    window.listBox1.Items.Add(requirementsstring);
                    window.listBox1.Items.Add(actionsstring);

                    string[] parsed_requirements = requirementsstring.Split('\t');
                    string[] parsed_actions = actionsstring.Split('\t');

                    string[] parsed2_requirements = parsed_requirements[1].Split(',');
                    string[] parsed2_actions = parsed_actions[1].Split(',');
                    List<string> statesList = parsed2_requirements.ToList();

                    List<string> actionsList = parsed2_actions.ToList();
                    Logic.rulesList.Add(new Rules(namestring, statesList, actionsList));

                }

            }
        }

        public void create_regex_versions()
        {

            foreach (string stringuru in this.conditionList)
            {
                if (new Regex(@"^<>.+").IsMatch(stringuru))
                {
                    this.special = true;
                    regexList.Add(new Regex(@"^<>.+"));
                }
                else
                {
                    string aa = stringuru.Replace(@"?", @"\?");
                    aa = aa.TrimEnd(aa[aa.Length - 1]);
                    Regex regex = new Regex(aa);
                    aa = Regex.Replace(aa, @"\\\?[^\\s]", "[A-Za-z]+");
                    regex = new Regex(aa);
                    regexList.Add(regex);
                }
            }
        }

        public void create_all_maping()
        {
            this.VariablesList.Clear();
            this.finalList.Clear();
            int statenemtCounteru = 0;
            int variablemapCounteru = 0;
            foreach (Regex regex in regexList)
            {
                this.VariablesList.Add(new List<Dictionary<string, string>>());
                variablemapCounteru = 0;
                foreach (Facts fact in Logic.factsList)
                {
                    if (regex.IsMatch(fact.getinfo()))
                    {
                        this.VariablesList[statenemtCounteru].Add(new Dictionary<string, string>());
                        string condition = this.conditionList[statenemtCounteru];
                        int finalcounteru = 0;
                        for (int i = condition.IndexOf("?"); i >= 0; i = condition.IndexOf("?", i + 1))
                        {
                            string duck = "" + condition[i + 1];
                            List<string> nameList = Facts.get_unknowns(fact.getinfo());
                            this.VariablesList[statenemtCounteru][variablemapCounteru].Add(duck, nameList[finalcounteru]);
                            finalcounteru = finalcounteru + 1;
                        }
                        variablemapCounteru = variablemapCounteru + 1;
                    }


                }
                statenemtCounteru = statenemtCounteru + 1;
            }
        }


        public void remove_bad_mappings()
        {
            List<List<Dictionary<string, string>>> za_list = this.VariablesList;

            int number_of_possibilities = za_list[0].Count;
            number_of_possibilities = za_list.Where(listt => listt.Count != 0)
                .Aggregate(number_of_possibilities, (current, listt) => current*listt.Count);

            for (; number_of_possibilities > 0; number_of_possibilities--)
            {
                List<Dictionary<string, string>> thisList = new List<Dictionary<string, string>>();
                int poz = number_of_possibilities;
                foreach (List<Dictionary<string, string>> list in za_list)
                {
                    if (list.Count != 0)
                    {
                        thisList.Add(list[poz%list.Count]);
                        poz = poz/list.Count;
                    }
                }

                Boolean res = true;
                Dictionary<string, string> actualDictionary = new Dictionary<string, string>();
                for (int i = 0; i < thisList.Count; i++)
                {
                    foreach (KeyValuePair<string, string> key in thisList[i])
                    {
                        if (!actualDictionary.ContainsKey(key.Key))
                        {
                            actualDictionary.Add(key.Key, key.Value);
                        }
                        else if (!actualDictionary[key.Key].SequenceEqual(key.Value))
                        {
                            res = false;
                            break;
                        }

                    }

                }
                if (res)
                {
                    this.finalList.Add(actualDictionary);
                }

            }
            if (this.special)
            {
                specialrule();
            }
        }


        public void specialrule()
        {
            string[] variab = new string[2];
            foreach (string s in this.conditionList)
            {
                if (Regex.IsMatch(s, @"^<>.+"))
                {
                    int counteru = 0;
                    for (int i = s.IndexOf('?'); i >= 0; i = s.IndexOf('?', i + 1))
                    {
                        variab[counteru] = "" + s[i + 1];
                        counteru++;
                    }

                }
            }
            HashSet<int> delete = new HashSet<int>();

            for (int i = 0; i < this.finalList.Count; i++)
            {
                int ekerualu = 0;
                Dictionary<string, string> annoyedDictionary = this.finalList[i];
                if (annoyedDictionary.ContainsKey(variab[0]))
                {
                    ekerualu++;
                }
                if (annoyedDictionary.ContainsKey(variab[1]))
                {
                    ekerualu++;
                }
                if (ekerualu == 2 && annoyedDictionary[variab[0]].SequenceEqual(annoyedDictionary[variab[1]]))
                {
                    delete.Add(i);
                }

            }
            List<Dictionary<string, string>> helpmeList = new List<Dictionary<string, string>>();
            for (int i = 0; i < this.finalList.Count; i++)
            {
                if (!delete.Contains(i))
                {
                    helpmeList.Add(this.finalList[i]);
                }
            }
            this.finalList.Clear();
            this.finalList = helpmeList;

        }

        public void doit()
        {
            foreach (string sao in actionList)
            {
                string act = sao;
                List<string> news = new List<string>();
                string helpmestring = "";
                for (int i = 0; i < this.finalList.Count; i++)
                {
                    helpmestring = act;
                    for (int j = act.IndexOf('?'); j >= 0; j = act.IndexOf('?', j + 1))
                    {
                        string unknow = "" + act[j + 1];
                        string full = "?" + unknow;
                        try
                        {
                            helpmestring = helpmestring.Replace(full, this.finalList[i][unknow]);
                        }
                        catch (Exception e)
                        {


                        }

                    }
                    Logic.actionsList.Add(helpmestring);
                }

            }
        }

        public static Boolean justdoit(MainWindow window)
        {
            foreach (string stringuru in Logic.actionsList)
            {
                string[] splited = stringuru.Split(' ');
                string fact = "";
                for (int i = 1; i < splited.Length; i++)
                {
                    if (i == splited.Length - 1)
                    {
                        fact = fact + splited[i];
                    }
                    else
                    {
                        fact = fact + splited[i] + " ";
                    }
                }

                if (splited[0].SequenceEqual("pridaj"))
                {
                    if (!Facts.issexy(fact))
                    {
                        Logic.factsList.Add(new Facts(fact));
                        Rules.print(fact, window);
                        return true;

                    }
                }
                if (splited[0].SequenceEqual("vymaz"))
                {
                    if (Facts.issexy(fact))
                    {
                        Logic.factsList.Remove(Logic.factsList[Facts.factPosition(fact)]);
                        Facts.add(window);
                        return true;
                    }
                }
                if (splited[0].SequenceEqual("sprava"))
                {
                    if (!window.listBox2.Items.Contains(fact))
                    {
                        window.listBox2.Items.Add(fact);
                        return true;
                    }
                    
                   
                }
            }
            return false;
        }

        public static void print(string act, MainWindow window)
        {
            window.listBox.Items.Add(act);
        }

    }
}