-- Team Management Client

local currentTeam = nil
local teamMembers = {}

-- Event: Team created
RegisterNetEvent('cbps:teamCreated')
AddEventHandler('cbps:teamCreated', function(teamId, teamName)
    currentTeam = {id = teamId, name = teamName}
    ShowNotification('~g~Team created: ' .. teamName)
end)

-- Event: Joined team
RegisterNetEvent('cbps:joinedTeam')
AddEventHandler('cbps:joinedTeam', function(teamId, teamName, members)
    currentTeam = {id = teamId, name = teamName}
    teamMembers = members
    ShowNotification('~g~Joined team: ' .. teamName)
end)

-- Event: Left team
RegisterNetEvent('cbps:leftTeam')
AddEventHandler('cbps:leftTeam', function()
    currentTeam = nil
    teamMembers = {}
    ShowNotification('~r~Left team')
end)

-- Event: Player joined team
RegisterNetEvent('cbps:playerJoinedTeam')
AddEventHandler('cbps:playerJoinedTeam', function(playerName)
    ShowNotification('~b~' .. playerName .. ' joined the team')
end)

-- Event: Player left team
RegisterNetEvent('cbps:playerLeftTeam')
AddEventHandler('cbps:playerLeftTeam', function(playerName)
    ShowNotification('~r~' .. playerName .. ' left the team')
end)

-- Event: Team chat message
RegisterNetEvent('cbps:teamChatMessage')
AddEventHandler('cbps:teamChatMessage', function(playerName, message)
    TriggerEvent('chat:addMessage', {
        color = {0, 255, 0},
        multiline = true,
        args = {'[TEAM] ' .. playerName, message}
    })
end)

-- Event: Team invitation
RegisterNetEvent('cbps:teamInvitation')
AddEventHandler('cbps:teamInvitation', function(teamId, teamName, inviterName)
    -- Show notification with invitation
    ShowNotification('~b~Team invitation from ' .. inviterName .. ' to join ' .. teamName)
    ShowNotification('~y~Press Y to accept, N to decline')
    
    -- Wait for player response
    Citizen.CreateThread(function()
        local timeout = GetGameTimer() + 30000 -- 30 second timeout
        
        while GetGameTimer() < timeout do
            Citizen.Wait(0)
            
            if IsControlJustReleased(0, 246) then -- Y key
                TriggerServerEvent('cbps:acceptTeamInvite', teamId)
                break
            elseif IsControlJustReleased(0, 306) then -- N key
                TriggerServerEvent('cbps:declineTeamInvite', teamId)
                ShowNotification('~r~Team invitation declined')
                break
            end
        end
    end)
end)

-- Update team member list
RegisterNetEvent('cbps:updateTeamMembers')
AddEventHandler('cbps:updateTeamMembers', function(members)
    teamMembers = members
end)

-- Export functions
exports('GetCurrentTeam', function()
    return currentTeam
end)

exports('GetTeamMembers', function()
    return teamMembers
end)
