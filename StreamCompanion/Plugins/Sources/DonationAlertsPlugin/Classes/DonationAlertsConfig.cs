using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompanionPlugin.Interfaces;

namespace DonationAlertsPlugin.Classes;

public class DonationAlertsConfig : IServiceSettings
{
    public bool Enabled { get; set; }
}