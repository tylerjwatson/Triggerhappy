# Triggerhappy for TSAPI

Triggerhappy for TSAPI is a mid-level packet filtering plugin that forces all Terraria packets specified through a firewall system in the plugins called "trigger chains".

Trigger chains are completely configurable and can subject a particular packet to scrutiny and sanity checks that the regular server cannot, thus protecting the server from various client-sided hacks.

## What are chains?

Put simply, chains are a group of commands.  Chains have Filters which specify if a chain is relevant to a particular packet, Triggers, which perform inspections and other things on a packet, and Actions which get performed when a trigger gets "pulled".

### What's a trigger "pull"?

Triggers will 'pull', or activate, when their sanity checks have failed and the chain's actions are to be performed.  For example, the PacketThresold trigger will pull if its threshold for a certain packet type from a particular player has been exceeded, forcing the chain to disconnect a player, ban, kick, or whatever the chain has specified in the action.
