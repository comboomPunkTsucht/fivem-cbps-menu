-- Player Options
local godMode = false
local invisible = false
local noclip = false
local superJump = false
local fastRun = false

function HealPlayer()
    local playerPed = PlayerPedId()
    SetEntityHealth(playerPed, 200)
    ShowNotification('~g~Health restored!')
end

function GiveArmor()
    local playerPed = PlayerPedId()
    SetPedArmour(playerPed, 100)
    ShowNotification('~b~Armor restored!')
end

function ToggleGodMode()
    godMode = not godMode
    local playerPed = PlayerPedId()
    SetEntityInvincible(playerPed, godMode)
    ShowNotification(godMode and '~g~God Mode: ON' or '~r~God Mode: OFF')
end

function ToggleInvisible()
    invisible = not invisible
    local playerPed = PlayerPedId()
    SetEntityVisible(playerPed, not invisible, 0)
    ShowNotification(invisible and '~g~Invisible: ON' or '~r~Invisible: OFF')
end

function ToggleNoclip()
    noclip = not noclip
    local playerPed = PlayerPedId()
    
    if noclip then
        SetEntityInvincible(playerPed, true)
        SetEntityVisible(playerPed, false, 0)
        ShowNotification('~g~Noclip: ON')
        
        Citizen.CreateThread(function()
            while noclip do
                Citizen.Wait(0)
                
                local coords = GetEntityCoords(playerPed)
                local heading = GetEntityHeading(playerPed)
                local speed = 1.0
                
                if IsControlPressed(0, 21) then -- Shift
                    speed = 5.0
                end
                
                if IsControlPressed(0, 32) then -- W
                    coords = GetOffsetFromEntityInOrientation(playerPed, 0.0, speed, 0.0)
                    SetEntityCoords(playerPed, coords.x, coords.y, coords.z, 0, 0, 0, false)
                end
                
                if IsControlPressed(0, 33) then -- S
                    coords = GetOffsetFromEntityInOrientation(playerPed, 0.0, -speed, 0.0)
                    SetEntityCoords(playerPed, coords.x, coords.y, coords.z, 0, 0, 0, false)
                end
                
                if IsControlPressed(0, 34) then -- A
                    SetEntityHeading(playerPed, heading + 3.0)
                end
                
                if IsControlPressed(0, 35) then -- D
                    SetEntityHeading(playerPed, heading - 3.0)
                end
                
                if IsControlPressed(0, 44) then -- Q
                    coords = GetOffsetFromEntityInOrientation(playerPed, 0.0, 0.0, -speed)
                    SetEntityCoords(playerPed, coords.x, coords.y, coords.z, 0, 0, 0, false)
                end
                
                if IsControlPressed(0, 38) then -- E
                    coords = GetOffsetFromEntityInOrientation(playerPed, 0.0, 0.0, speed)
                    SetEntityCoords(playerPed, coords.x, coords.y, coords.z, 0, 0, 0, false)
                end
            end
        end)
    else
        SetEntityInvincible(playerPed, godMode)
        SetEntityVisible(playerPed, not invisible, 0)
        ShowNotification('~r~Noclip: OFF')
    end
end

function GetOffsetFromEntityInOrientation(entity, offsetX, offsetY, offsetZ)
    local pos = GetEntityCoords(entity)
    local rot = GetEntityRotation(entity, 2)
    local forward = GetForwardVector(rot)
    local right = GetRightVector(rot)
    local up = vector3(0.0, 0.0, 1.0)
    
    return vector3(
        pos.x + (right.x * offsetX) + (forward.x * offsetY) + (up.x * offsetZ),
        pos.y + (right.y * offsetX) + (forward.y * offsetY) + (up.y * offsetZ),
        pos.z + (right.z * offsetX) + (forward.z * offsetY) + (up.z * offsetZ)
    )
end

function GetForwardVector(rotation)
    local rot = (rotation * 3.14159265359) / 180.0
    return vector3(-math.sin(rot.z), math.cos(rot.z), 0.0)
end

function GetRightVector(rotation)
    local rot = (rotation * 3.14159265359) / 180.0
    return vector3(math.cos(rot.z), math.sin(rot.z), 0.0)
end

function ToggleSuperJump()
    superJump = not superJump
    ShowNotification(superJump and '~g~Super Jump: ON' or '~r~Super Jump: OFF')
    
    if superJump then
        Citizen.CreateThread(function()
            while superJump do
                Citizen.Wait(0)
                SetSuperJumpThisFrame(PlayerId())
            end
        end)
    end
end

function ToggleFastRun()
    fastRun = not fastRun
    ShowNotification(fastRun and '~g~Fast Run: ON' or '~r~Fast Run: OFF')
    
    if fastRun then
        Citizen.CreateThread(function()
            while fastRun do
                Citizen.Wait(0)
                SetRunSprintMultiplierForPlayer(PlayerId(), 1.49)
            end
        end)
    else
        SetRunSprintMultiplierForPlayer(PlayerId(), 1.0)
    end
end

function SuicidePlayer()
    local playerPed = PlayerPedId()
    SetEntityHealth(playerPed, 0)
    ShowNotification('~r~You have committed suicide')
end

function ClearWantedLevel()
    local playerId = PlayerId()
    SetPlayerWantedLevel(playerId, 0, false)
    SetPlayerWantedLevelNow(playerId, false)
    ShowNotification('~g~Wanted level cleared')
end

function TeleportToWaypoint()
    local waypoint = GetFirstBlipInfoId(8)
    
    if DoesBlipExist(waypoint) then
        local coords = GetBlipInfoIdCoord(waypoint)
        local playerPed = PlayerPedId()
        
        -- Get ground Z coordinate
        local groundZ = 0.0
        local found, z = GetGroundZFor_3dCoord(coords.x, coords.y, 1000.0, false)
        
        if found then
            groundZ = z
        else
            groundZ = coords.z
        end
        
        SetEntityCoords(playerPed, coords.x, coords.y, groundZ, 0, 0, 0, false)
        ShowNotification('~g~Teleported to waypoint')
    else
        ShowNotification('~r~No waypoint set')
    end
end
