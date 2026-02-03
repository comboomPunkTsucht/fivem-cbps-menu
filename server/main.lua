-- Server Main Script

print('^2[CBPS Menu] ^7Starting server...')

-- Player join event
AddEventHandler('playerConnecting', function(name, setKickReason, deferrals)
    local playerId = source
    print('^2[CBPS Menu] ^7Player ' .. name .. ' connecting...')
end)

AddEventHandler('playerDropped', function(reason)
    local playerId = source
    print('^2[CBPS Menu] ^7Player dropped: ' .. reason)
    
    -- Clean up player data
    TriggerEvent('cbps:playerDropped', playerId)
end)

print('^2[CBPS Menu] ^7Server started successfully!')
