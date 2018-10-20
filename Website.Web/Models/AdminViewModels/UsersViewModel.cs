﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Service.DTO;

namespace Website.Web.Models.AdminViewModels
{
    public class UsersViewModel
    {
        #region page state properties
        public string CurrentSearch { get; set; }
        public string CurrentSortOrder { get; set; }
        public bool Descending { get; set; }
        public int CurrentPage { get; set; }
        public int CountPerPage { get; set; }
        public int Roles { get; set; }
        public int ItemCount { get; set; }
        #endregion

        public IEnumerable<UserDTO> users { get; set; }
    }
}