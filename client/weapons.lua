-- Weapon Options

-- Give weapon event
RegisterNetEvent('cbps:giveWeapon')
AddEventHandler('cbps:giveWeapon', function(weapon)
    GiveWeaponToPed(weapon)
end)

function GiveWeaponToPed(weapon)
    local playerPed = PlayerPedId()
    GiveWeaponToPed(playerPed, GetHashKey(weapon), 250, false, true)
    ShowNotification('~g~Weapon given: ' .. weapon)
end

function GiveAllWeapons()
    local playerPed = PlayerPedId()
    
    for _, category in pairs(Config.WeaponCategories) do
        for _, weapon in pairs(category.weapons) do
            GiveWeaponToPed(playerPed, GetHashKey(weapon), 250, false, false)
        end
    end
    
    ShowNotification('~g~All weapons given')
end

function RemoveAllWeapons()
    local playerPed = PlayerPedId()
    RemoveAllPedWeapons(playerPed, true)
    ShowNotification('~r~All weapons removed')
end

local infiniteAmmo = false
function ToggleInfiniteAmmo()
    infiniteAmmo = not infiniteAmmo
    ShowNotification(infiniteAmmo and '~g~Infinite Ammo: ON' or '~r~Infinite Ammo: OFF')
    
    if infiniteAmmo then
        Citizen.CreateThread(function()
            while infiniteAmmo do
                Citizen.Wait(0)
                SetPedInfiniteAmmoClip(PlayerPedId(), true)
            end
        end)
    else
        SetPedInfiniteAmmoClip(PlayerPedId(), false)
    end
end

local noReload = false
function ToggleNoReload()
    noReload = not noReload
    local playerPed = PlayerPedId()
    
    if noReload then
        -- Get current weapon
        local _, currentWeapon = GetCurrentPedWeapon(playerPed, true)
        SetPedAmmo(playerPed, currentWeapon, 9999)
        ShowNotification('~g~No Reload: ON')
        
        Citizen.CreateThread(function()
            while noReload do
                Citizen.Wait(0)
                local ped = PlayerPedId()
                local _, weapon = GetCurrentPedWeapon(ped, true)
                SetPedAmmo(ped, weapon, 9999)
            end
        end)
    else
        ShowNotification('~r~No Reload: OFF')
    end
end
