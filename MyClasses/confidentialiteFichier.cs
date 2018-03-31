using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyClasses
{
    public class ConfidentialiteFichier
    {
        private List<Utilisateur> utilisateurs;
        private List<Groupe> groups;

        public ConfidentialiteFichier(List<Utilisateur> lu, List<Groupe> lg)
        {
            this.utilisateurs = lu;
            this.groups = lg;
        }
        public List<Utilisateur> getListUtil()
        { return utilisateurs; }
        public List<Groupe> getListGroup()
        { return groups; }
    }
}
