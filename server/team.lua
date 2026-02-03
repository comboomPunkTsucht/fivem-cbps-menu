-- Server Team Management

local teams = {}
local playerTeams = {}
local nextTeamId = 1

-- Create team
RegisterNetEvent('cbps:createTeam')
AddEventHandler('cbps:createTeam', function()
    local playerId = source
    local playerName = GetPlayerName(playerId)
    
    -- Check if player is already in a team
    if playerTeams[playerId] then
        TriggerClientEvent('cbps:leftTeam', playerId)
    end
    
    -- Create new team
    local teamId = nextTeamId
    nextTeamId = nextTeamId + 1
    
    local teamName = playerName .. "'s Team"
    local colorIndex = (teamId % #Config.Team.TeamColors) + 1
    
    teams[teamId] = {
        id = teamId,
        name = teamName,
        leader = playerId,
        members = {playerId},
        color = Config.Team.TeamColors[colorIndex],
        createdAt = os.time()
    }
    
    playerTeams[playerId] = teamId
    
    TriggerClientEvent('cbps:teamCreated', playerId, teamId, teamName)
    print('[CBPS Menu] Team created: ' .. teamName .. ' (ID: ' .. teamId .. ')')
end)

-- Leave team
RegisterNetEvent('cbps:leaveTeam')
AddEventHandler('cbps:leaveTeam', function()
    local playerId = source
    local teamId = playerTeams[playerId]
    
    if not teamId then
        return
    end
    
    local team = teams[teamId]
    if not team then
        return
    end
    
    -- Remove player from team
    for i, memberId in ipairs(team.members) do
        if memberId == playerId then
            table.remove(team.members, i)
            break
        end
    end
    
    playerTeams[playerId] = nil
    
    -- Notify all team members
    for _, memberId in ipairs(team.members) do
        TriggerClientEvent('cbps:playerLeftTeam', memberId, GetPlayerName(playerId))
    end
    
    TriggerClientEvent('cbps:leftTeam', playerId)
    
    -- Delete team if empty or leader left
    if #team.members == 0 or team.leader == playerId then
        for _, memberId in ipairs(team.members) do
            TriggerClientEvent('cbps:leftTeam', memberId)
            playerTeams[memberId] = nil
        end
        teams[teamId] = nil
        print('[CBPS Menu] Team deleted: ' .. team.name)
    end
end)

-- Invite nearby player
RegisterNetEvent('cbps:inviteNearbyPlayer')
AddEventHandler('cbps:inviteNearbyPlayer', function()
    local playerId = source
    local teamId = playerTeams[playerId]
    
    if not teamId then
        return
    end
    
    local team = teams[teamId]
    if not team then
        return
    end
    
    -- Check if team is full
    if #team.members >= Config.Team.MaxTeamSize then
        TriggerClientEvent('cbps:showNotification', playerId, '~r~Team is full')
        return
    end
    
    -- Find nearest player
    local playerCoords = GetEntityCoords(GetPlayerPed(playerId))
    local nearestPlayer = nil
    local nearestDistance = 10.0
    
    for _, targetId in ipairs(GetPlayers()) do
        targetId = tonumber(targetId)
        if targetId ~= playerId and not playerTeams[targetId] then
            local targetCoords = GetEntityCoords(GetPlayerPed(targetId))
            local distance = #(playerCoords - targetCoords)
            
            if distance < nearestDistance then
                nearestPlayer = targetId
                nearestDistance = distance
            end
        end
    end
    
    if nearestPlayer then
        TriggerClientEvent('cbps:teamInvitation', nearestPlayer, teamId, team.name, GetPlayerName(playerId))
    end
end)

-- Accept team invite
RegisterNetEvent('cbps:acceptTeamInvite')
AddEventHandler('cbps:acceptTeamInvite', function(teamId)
    local playerId = source
    local team = teams[teamId]
    
    if not team then
        return
    end
    
    -- Check if team is full
    if #team.members >= Config.Team.MaxTeamSize then
        TriggerClientEvent('cbps:showNotification', playerId, '~r~Team is full')
        return
    end
    
    -- Add player to team
    table.insert(team.members, playerId)
    playerTeams[playerId] = teamId
    
    -- Notify all team members
    for _, memberId in ipairs(team.members) do
        if memberId ~= playerId then
            TriggerClientEvent('cbps:playerJoinedTeam', memberId, GetPlayerName(playerId))
        end
    end
    
    TriggerClientEvent('cbps:joinedTeam', playerId, teamId, team.name, team.members)
end)

-- Decline team invite
RegisterNetEvent('cbps:declineTeamInvite')
AddEventHandler('cbps:declineTeamInvite', function(teamId)
    -- Nothing to do
end)

-- Team chat
RegisterNetEvent('cbps:teamChat')
AddEventHandler('cbps:teamChat', function(message)
    local playerId = source
    local teamId = playerTeams[playerId]
    
    if not teamId then
        return
    end
    
    local team = teams[teamId]
    if not team then
        return
    end
    
    local playerName = GetPlayerName(playerId)
    
    -- Send message to all team members
    for _, memberId in ipairs(team.members) do
        TriggerClientEvent('cbps:teamChatMessage', memberId, playerName, message)
    end
end)

-- Player dropped - clean up team
AddEventHandler('cbps:playerDropped', function(playerId)
    local teamId = playerTeams[playerId]
    if teamId then
        local team = teams[teamId]
        if team then
            -- Remove player from team
            for i, memberId in ipairs(team.members) do
                if memberId == playerId then
                    table.remove(team.members, i)
                    break
                end
            end
            
            -- Notify all team members
            for _, memberId in ipairs(team.members) do
                TriggerClientEvent('cbps:playerLeftTeam', memberId, GetPlayerName(playerId))
            end
            
            -- Delete team if empty or leader left
            if #team.members == 0 or team.leader == playerId then
                for _, memberId in ipairs(team.members) do
                    TriggerClientEvent('cbps:leftTeam', memberId)
                    playerTeams[memberId] = nil
                end
                teams[teamId] = nil
            end
        end
        
        playerTeams[playerId] = nil
    end
end)
