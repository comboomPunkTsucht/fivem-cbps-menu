using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CitizenFX.Core;

using LemonUI;
using LemonUI.Menus;

using CBPSMenu.Shared;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Player Appearance submenu - vMenu clone.
    /// </summary>
    public class PlayerAppearance
    {
        private NativeMenu menu;
        private NativeMenu spawnPedMenu;
        private NativeMenu savedPedsMenu;
        private NativeMenu customizationMenu;

        // Ped categories for spawning
        private readonly Dictionary<string, List<string>> PedCategories = new Dictionary<string, List<string>>
        {
            { "Animals", new List<string> { "a_c_boar", "a_c_cat_01", "a_c_chickenhawk", "a_c_chimp", "a_c_chop", "a_c_cormorant", "a_c_cow", "a_c_coyote", "a_c_crow", "a_c_deer", "a_c_fish", "a_c_hen", "a_c_husky", "a_c_mtlion", "a_c_pig", "a_c_pigeon", "a_c_poodle", "a_c_pug", "a_c_rabbit_01", "a_c_rat", "a_c_retriever", "a_c_rhesus", "a_c_rottweiler", "a_c_seagull", "a_c_sharkhammer", "a_c_sharktiger", "a_c_shepherd", "a_c_westy" } },
            { "Ambient Female", new List<string> { "a_f_m_beach_01", "a_f_m_bevhills_01", "a_f_m_bevhills_02", "a_f_m_bodybuild_01", "a_f_m_business_02", "a_f_m_downtown_01", "a_f_m_eastsa_01", "a_f_m_eastsa_02", "a_f_m_fatbla_01", "a_f_m_fatcult_01", "a_f_m_fatwhite_01", "a_f_m_ktown_01", "a_f_m_ktown_02", "a_f_m_prolhost_01", "a_f_m_salton_01", "a_f_m_skidrow_01", "a_f_m_soucentmc_01", "a_f_m_soucent_01", "a_f_m_soucent_02", "a_f_m_tourist_01", "a_f_m_trampbeac_01", "a_f_m_tramp_01" } },
            { "Ambient Male", new List<string> { "a_m_m_acult_01", "a_m_m_afriamer_01", "a_m_m_beach_01", "a_m_m_beach_02", "a_m_m_bevhills_01", "a_m_m_bevhills_02", "a_m_m_business_01", "a_m_m_eastsa_01", "a_m_m_eastsa_02", "a_m_m_farmer_01", "a_m_m_fatlatin_01", "a_m_m_genfat_01", "a_m_m_genfat_02", "a_m_m_golfer_01", "a_m_m_hasjew_01", "a_m_m_hillbilly_01", "a_m_m_hillbilly_02", "a_m_m_indian_01", "a_m_m_ktown_01", "a_m_m_malibu_01", "a_m_m_mexcntry_01", "a_m_m_mexlabor_01" } },
            { "Cutscene", new List<string> { "cs_amandatownley", "cs_andreas", "cs_ashley", "cs_bankman", "cs_barry", "cs_beverly", "cs_brad", "cs_bradcadaver", "cs_carbuyer", "cs_casey", "cs_chengsr", "cs_chrisformage", "cs_clay", "cs_dale", "cs_davenorton", "cs_debra", "cs_denise", "cs_devin", "cs_dom", "cs_dreyfuss", "cs_drfriedlander", "cs_fabien", "cs_fbisuit_01", "cs_floyd", "cs_guadalope", "cs_gurk", "cs_hunter", "cs_janet", "cs_jewelass", "cs_jimmyboston" } },
            { "Gang Female", new List<string> { "g_f_y_ballas_01", "g_f_y_families_01", "g_f_y_lost_01", "g_f_y_vagos_01" } },
            { "Gang Male", new List<string> { "g_m_m_armboss_01", "g_m_m_armgoon_01", "g_m_m_armlieut_01", "g_m_m_chemwork_01", "g_m_m_chiboss_01", "g_m_m_chicold_01", "g_m_m_chigoon_01", "g_m_m_chigoon_02", "g_m_m_korboss_01", "g_m_m_mexboss_01", "g_m_m_mexboss_02", "g_m_y_armgoon_02", "g_m_y_azteca_01", "g_m_y_ballaeast_01", "g_m_y_ballaorig_01", "g_m_y_ballasout_01", "g_m_y_famca_01", "g_m_y_famdnf_01", "g_m_y_famfor_01", "g_m_y_korean_01", "g_m_y_korean_02", "g_m_y_korlieut_01", "g_m_y_lost_01", "g_m_y_lost_02", "g_m_y_lost_03", "g_m_y_mexgang_01", "g_m_y_mexgoon_01", "g_m_y_mexgoon_02", "g_m_y_mexgoon_03", "g_m_y_pologoon_01", "g_m_y_pologoon_02", "g_m_y_salvaboss_01", "g_m_y_salvagoon_01", "g_m_y_salvagoon_02", "g_m_y_salvagoon_03", "g_m_y_strpunk_01", "g_m_y_strpunk_02" } },
            { "Story", new List<string> { "player_zero", "player_one", "player_two", "ig_abigail", "ig_amandatownley", "ig_andreas", "ig_ashley", "ig_ballasog", "ig_bankman", "ig_barry", "ig_bestmen", "ig_beverly", "ig_brad", "ig_bride", "ig_car3guy1", "ig_car3guy2", "ig_casey", "ig_chef", "ig_chengsr", "ig_chrisformage", "ig_clay", "ig_claypain", "ig_cletus", "ig_dale", "ig_davenorton", "ig_denise", "ig_devin", "ig_dom", "ig_dreyfuss", "ig_drfriedlander", "ig_fabien", "ig_fbisuit_01", "ig_floyd", "ig_groom", "ig_hao", "ig_hunter", "ig_janet", "ig_jay_norris", "ig_jewelass", "ig_jimmyboston", "ig_jimmydisanto", "ig_joeminuteman", "ig_johnnyklebitz", "ig_josef", "ig_josh", "ig_kerrymcintosh", "ig_lamardavis", "ig_lazlow", "ig_lestercrest", "ig_lifeinvad_01", "ig_lifeinvad_02", "ig_magenta", "ig_manuel", "ig_marnie", "ig_maryann", "ig_maude", "ig_michelle", "ig_milton", "ig_molly", "ig_mrk", "ig_mrsphillips", "ig_mrs_thornhill", "ig_natalia", "ig_nervousron", "ig_nigel", "ig_old_man1a", "ig_old_man2", "ig_omega", "ig_oneil", "ig_orleans", "ig_ortega", "ig_paper", "ig_patricia", "ig_priest", "ig_prolsec_02", "ig_ramp_gang", "ig_ramp_hic", "ig_ramp_hipster", "ig_ramp_mex", "ig_roccopelosi", "ig_russiandrunk", "ig_screen_writer", "ig_siemonyetarian", "ig_solomon", "ig_stevehains", "ig_stretch", "ig_talina", "ig_tanisha", "ig_taocheng", "ig_taostranslator", "ig_tenniscoach", "ig_terry", "ig_tomepsilon", "ig_tonya", "ig_tracydisanto", "ig_tylerdix", "ig_wade", "ig_zimbor" } },
            { "Scenario Female", new List<string> { "s_f_m_fembarber", "s_f_m_maid_01", "s_f_m_shop_high", "s_f_m_sweatshop_01", "s_f_y_airhostess_01", "s_f_y_bartender_01", "s_f_y_baywatch_01", "s_f_y_cop_01", "s_f_y_factory_01", "s_f_y_hooker_01", "s_f_y_hooker_02", "s_f_y_hooker_03", "s_f_y_migrant_01", "s_f_y_movprem_01", "s_f_y_ranger_01", "s_f_y_scrubs_01", "s_f_y_sheriff_01", "s_f_y_shop_low", "s_f_y_shop_mid", "s_f_y_stripper_01", "s_f_y_stripper_02", "s_f_y_sweatshop_01" } },
            { "Scenario Male", new List<string> { "s_m_m_ammucountry", "s_m_m_armoured_01", "s_m_m_armoured_02", "s_m_m_autoshop_01", "s_m_m_autoshop_02", "s_m_m_bouncer_01", "s_m_m_chemsec_01", "s_m_m_ciasec_01", "s_m_m_cntrybar_01", "s_m_m_dockwork_01", "s_m_m_doctor_01", "s_m_m_fiboffice_01", "s_m_m_fiboffice_02", "s_m_m_gaffer_01", "s_m_m_gardener_01", "s_m_m_gentransport", "s_m_m_hairdress_01", "s_m_m_highsec_01", "s_m_m_highsec_02", "s_m_m_janitor", "s_m_m_lathandy_01", "s_m_m_lifeinvad_01", "s_m_m_linecook", "s_m_m_lsmetro_01", "s_m_m_mariachi_01", "s_m_m_marine_01", "s_m_m_marine_02", "s_m_m_migrant_01", "s_m_m_movalien_01", "s_m_m_movprem_01", "s_m_m_movspace_01", "s_m_m_paramedic_01", "s_m_m_pilot_01", "s_m_m_pilot_02", "s_m_m_postal_01", "s_m_m_postal_02", "s_m_m_prisguard_01", "s_m_m_scientist_01", "s_m_m_security_01", "s_m_m_snowcop_01", "s_m_m_strperf_01", "s_m_m_strpreach_01", "s_m_m_strvend_01", "s_m_m_trucker_01", "s_m_m_ups_01", "s_m_m_ups_02" } },
            { "Emergency", new List<string> { "s_m_y_cop_01", "s_f_y_cop_01", "s_m_y_hwaycop_01", "s_m_y_sheriff_01", "s_f_y_sheriff_01", "s_m_y_ranger_01", "s_f_y_ranger_01", "s_m_m_paramedic_01", "s_m_y_fireman_01", "s_m_m_snowcop_01", "s_m_y_swat_01", "s_m_m_fiboffice_01", "s_m_m_fiboffice_02", "s_m_m_ciasec_01", "s_m_y_marine_01", "s_m_y_marine_02", "s_m_y_marine_03", "s_m_m_marine_01", "s_m_m_marine_02", "s_m_y_armymech_01", "s_m_y_blackops_01", "s_m_y_blackops_02", "s_m_y_blackops_03" } },
        };

        private void CreateMenu()
        {
            menu = new NativeMenu("Player Appearance", "Change Your Appearance");
            spawnPedMenu = new NativeMenu("Spawn as Ped", "Select a Ped Model");
            savedPedsMenu = new NativeMenu("Saved Peds", "Your Saved Character Presets");
            customizationMenu = new NativeMenu("Customize Ped", "Customize Your Character");

            #region Spawn Ped

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.PASpawnPed))
            {
                var spawnPedBtn = new NativeItem("Spawn as Ped", "Change your ped model.") { AltTitle = "→→→" };
                menu.Add(spawnPedBtn);

                // Player models
                var playerModels = new NativeItem("~g~Default Characters", "Spawn as Michael, Franklin, or Trevor.");
                playerModels.Activated += async (s, e) =>
                {
                    await SpawnPed("player_zero"); // Michael
                };
                spawnPedMenu.Add(playerModels);

                var michaelBtn = new NativeItem("Michael", "Spawn as Michael De Santa.");
                michaelBtn.Activated += async (s, e) => await SpawnPed("player_zero");
                spawnPedMenu.Add(michaelBtn);

                var franklinBtn = new NativeItem("Franklin", "Spawn as Franklin Clinton.");
                franklinBtn.Activated += async (s, e) => await SpawnPed("player_one");
                spawnPedMenu.Add(franklinBtn);

                var trevorBtn = new NativeItem("Trevor", "Spawn as Trevor Philips.");
                trevorBtn.Activated += async (s, e) => await SpawnPed("player_two");
                spawnPedMenu.Add(trevorBtn);

                // Ped categories
                foreach (var category in PedCategories)
                {
                    var categoryMenu = new NativeMenu(category.Key, $"{category.Key} Peds");
                    var categoryBtn = new NativeItem(category.Key, $"Browse {category.Key.ToLower()} peds.") { AltTitle = "→→→" };
                    spawnPedMenu.Add(categoryBtn);

                    foreach (var ped in category.Value)
                    {
                        var pedName = ped;
                        var pedBtn = new NativeItem(FormatPedName(ped), $"Spawn as {ped}.");
                        pedBtn.Activated += async (s, e) => await SpawnPed(pedName);
                        categoryMenu.Add(pedBtn);
                    }
                }

                // Spawn by name
                var spawnByName = new NativeItem("Spawn by Name", "Enter a ped model name.");
                spawnByName.Activated += async (s, e) =>
                {
                    var input = await GetUserInput("Enter ped model name", "", 50);
                    if (!string.IsNullOrEmpty(input))
                    {
                        await SpawnPed(input.ToLower());
                    }
                };
                spawnPedMenu.Add(spawnByName);
            }

            #endregion

            #region Saved Peds

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.PASavedPeds))
            {
                var savedPedsBtn = new NativeItem("Saved Peds", "View your saved character presets.") { AltTitle = "→→→" };
                menu.Add(savedPedsBtn);

                var saveCurrent = new NativeItem("~g~Save Current Ped", "Save your current appearance.");
                saveCurrent.Activated += async (s, e) =>
                {
                    var name = await GetUserInput("Enter preset name", "", 30);
                    if (!string.IsNullOrEmpty(name))
                    {
                        // TODO: Save ped data to KVP or server
                        Notify.Success($"Saved ped as '{name}'.");
                    }
                };
                savedPedsMenu.Add(saveCurrent);
            }

            #endregion

            #region Customization

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.PACustomize))
            {
                var customizeBtn = new NativeItem("Customize Ped", "Change face, hair, accessories.") { AltTitle = "→→→" };
                menu.Add(customizeBtn);

                CreateCustomizationMenu();
            }

            #endregion

            #region Accessories

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.PAAccessories))
            {
                var accessoriesBtn = new NativeItem("Accessories", "Manage hats, glasses, earrings.");
                accessoriesBtn.Activated += (s, e) =>
                {
                    // Open component editing
                    Notify.Info("Use the customization menu for accessories.");
                };
                menu.Add(accessoriesBtn);
            }

            #endregion

            #region Reset Appearance

            var resetAppearance = new NativeItem("~r~Reset to Default", "Reset to your default online character.");
            resetAppearance.Activated += async (s, e) =>
            {
                await SpawnPed("mp_m_freemode_01");
                Notify.Success("Appearance reset.");
            };
            menu.Add(resetAppearance);

            #endregion
        }

        private void CreateCustomizationMenu()
        {
            // Component slots
            var componentNames = new string[] { "Head", "Beard/Mask", "Hair", "Torso", "Legs", "Bags/Parachute", "Shoes", "Accessory", "Undershirt", "Body Armor", "Decals", "Top" };

            for (var i = 0; i < componentNames.Length; i++)
            {
                var compIndex = i;
                var compName = componentNames[i];

                var compList = new NativeListItem<int>(compName, $"Change {compName.ToLower()}.", new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
                compList.ItemChanged += (s, e) =>
                {
                    var drawable = e.Object;
                    var maxDrawables = GetNumberOfPedDrawableVariations(Game.PlayerPed.Handle, compIndex);
                    if (drawable < maxDrawables)
                    {
                        SetPedComponentVariation(Game.PlayerPed.Handle, compIndex, drawable, 0, 0);
                    }
                };
                customizationMenu.Add(compList);
            }

            // Props (hats, glasses, etc.)
            var propNames = new string[] { "Hats", "Glasses", "Ears", "Watch", "Bracelet" };
            var propIndices = new int[] { 0, 1, 2, 6, 7 };

            for (var i = 0; i < propNames.Length; i++)
            {
                var propIndex = propIndices[i];
                var propName = propNames[i];

                var propList = new NativeListItem<int>(propName, $"Change {propName.ToLower()}.", new int[] { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
                propList.ItemChanged += (s, e) =>
                {
                    var drawable = e.Object;
                    if (drawable == -1)
                    {
                        ClearPedProp(Game.PlayerPed.Handle, propIndex);
                    }
                    else
                    {
                        var maxProps = GetNumberOfPedPropDrawableVariations(Game.PlayerPed.Handle, propIndex);
                        if (drawable < maxProps)
                        {
                            SetPedPropIndex(Game.PlayerPed.Handle, propIndex, drawable, 0, true);
                        }
                    }
                };
                customizationMenu.Add(propList);
            }

            // Clear all props
            var clearProps = new NativeItem("Clear All Props", "Remove all hats, glasses, etc.");
            clearProps.Activated += (s, e) =>
            {
                ClearAllPedProps(Game.PlayerPed.Handle);
                Notify.Success("All props cleared.");
            };
            customizationMenu.Add(clearProps);
        }

        private string FormatPedName(string pedModel)
        {
            // Convert "a_m_m_beach_01" to "Beach 01"
            var parts = pedModel.Split('_');
            if (parts.Length > 3)
            {
                var name = string.Join(" ", parts, 3, parts.Length - 3);
                return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.Replace("_", " "));
            }
            return pedModel;
        }

        private async Task SpawnPed(string modelName)
        {
            var modelHash = (uint)GetHashKey(modelName);

            if (!IsModelInCdimage(modelHash))
            {
                Notify.Error($"Model '{modelName}' not found.");
                return;
            }

            RequestModel(modelHash);
            while (!HasModelLoaded(modelHash))
            {
                await BaseScript.Delay(0);
            }

            SetPlayerModel(Game.Player.Handle, modelHash);
            SetModelAsNoLongerNeeded(modelHash);

            // Set default clothes for MP peds
            if (modelName == "mp_m_freemode_01" || modelName == "mp_f_freemode_01")
            {
                SetPedDefaultComponentVariation(Game.PlayerPed.Handle);
            }

            Notify.Success($"Spawned as {modelName}.");
        }

        private async Task<string> GetUserInput(string windowTitle, string defaultText, int maxLength)
        {
            AddTextEntry("FMMC_KEY_TIP1", windowTitle);
            DisplayOnscreenKeyboard(1, "FMMC_KEY_TIP1", "", defaultText, "", "", "", maxLength);
            while (UpdateOnscreenKeyboard() == 0)
            {
                await BaseScript.Delay(0);
            }
            if (UpdateOnscreenKeyboard() == 1)
            {
                return GetOnscreenKeyboardResult();
            }
            return null;
        }

        public NativeMenu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
            }
            return menu;
        }

        public NativeMenu GetSpawnPedMenu() => spawnPedMenu;
        public NativeMenu GetSavedPedsMenu() => savedPedsMenu;
        public NativeMenu GetCustomizationMenu() => customizationMenu;
    }
}
