﻿using ControlPanel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ControlPanel.Sourses
{
    class AdditionalInfoParent : PersonalUnit
    {
        private ClientModel ClientData {get;set;}
        public AdditionalInfoParent(ClientModel Client)
        {
            ClientData = Client;
        }
        public override Grid getGrid()
        {
            Grid Grid = new();
            Grid.RowDefinitions.Add(new RowDefinition());
            Grid.RowDefinitions.Add(new RowDefinition());
            string parentType = ClientData.ParentType != "" ? ClientData.ParentType : "Pодитель";
            string parentFIO = ClientData.ParentFIO.Trim(' ') != "" ? ClientData.ParentFIO : "не указано";
            string parentPhone = ClientData.ParentPhoneNumber != "" ? ClientData.ParentPhoneNumber : "не указано";

            // заголовок
            string infoParentHead = "Информация о родителях:";
            Label InfoParentHead = new Label()
            {
                Content = infoParentHead,
                FontWeight = FontWeights.Bold,
                Height = 25
            };
            Grid.Children.Add(InfoParentHead);
            Grid.SetRow(InfoParentHead, 0);

            // родитель
            string infoParent = $"{parentType}: {parentFIO}\nТелефон: {parentPhone}";
            
            Label InfoParent = new Label() { Content = infoParent };

            Grid.Children.Add(InfoParent);
            Grid.SetRow(InfoParent, 1);

            return Grid;
        }
    }
}
