using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace UI_zadanie_4_produkcny_system.Model
{
    public class Logic
    {
        public static List<Rules> rulesList=new List<Rules>();
        public static List<Facts> factsList=new List<Facts>();
        public static List<string> actionsList=new List<string>();

        public static void run(MainWindow window)
        {
            foreach (var rulese in rulesList)
            {
                    rulese.create_regex_versions();
            }


            while (true)
            {
                foreach (var rulese in rulesList)
                {
                        rulese.create_all_maping();
                        rulese.remove_bad_mappings();
                        rulese.doit();
                }

                if (Rules.justdoit(window) == false)
                {
                    break;
                }
                else
                {
                    actionsList.Clear();
                }
            }
        }
        
    }
}