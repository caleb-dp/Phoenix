using System;
using System.Collections.Generic;
using System.Text;
using Phoenix;
using Phoenix.WorldData;
using Phoenix.Communication;

namespace Phoenix.Scripts
{
    public class Morf
    {

//    Serial: 0x00322F52  Name: "Balron"  Position: 3388.423.29  Flags: 0x0000  Color: 0x0453  Model: 0x0025  Renamable: False Notoriety: Murderer HP: 256/256, Distance: 15

//Serial: 0x002AD084  Name: "Daemon Destroyer"  Position: 3385.432.9  Flags: 0x0000  Color: 0x0453  Model: 0x000A  Renamable: False Notoriety: Murderer HP: 256/256, Distance: 12

//Serial: 0x00367159  Name: "Bechard"  Position: 3392.413.9  Flags: 0x0000  Color: 0x0000  Model: 0x0009  Renamable: False Notoriety: Murderer HP: 234/234, Distance: 14


        private List<Serial> morfedCharacterSerials = new List<Serial>();
        private static Dictionary<Serial, ushort> morfedCharactersModels = new Dictionary<Serial, ushort>();
        private bool morfingRunning = false;
        private bool autoMorfingRunning = false;
        private bool selfMorfingRunning = false;

        [Command]
        public void SelfMorf()
        {
            if (selfMorfingRunning)
            {
                Core.UnregisterServerMessageCallback(0x20, OnUpdatePlayer);
                selfMorfingRunning = false;
                UO.PrintInformation("SelfMorfing vypnut!");
            }
            else
            {
                Core.RegisterServerMessageCallback(0x20, new MessageCallback(OnUpdatePlayer));
                UO.PrintInformation("SelfMorfing zapnut!");
                selfMorfingRunning = true;
            }
        }

        [Command]
        public void AutoMorfing()
        {
            if (autoMorfingRunning)
            {
                Core.UnregisterServerMessageCallback(0x77, OnAutoUpdateCharacter);
                autoMorfingRunning = false;
                UO.PrintInformation("Morfing vypnut!");
            }
            else
            {
                Core.RegisterServerMessageCallback(0x77, new MessageCallback(OnAutoUpdateCharacter));
                UO.PrintInformation("Morfing zapnut!");
                autoMorfingRunning = true;
            }
        }

        [Command]
        public void Morfing()
        {
            morfedCharacterSerials.Clear();
            morfedCharactersModels.Clear();
            if (morfingRunning)
            {
                Core.UnregisterServerMessageCallback(0x77, OnUpdateCharacter);
                morfingRunning = false;
                UO.PrintInformation("Morfing vypnut!");
            }
            else
            {
                Core.RegisterServerMessageCallback(0x77, new MessageCallback(OnUpdateCharacter));
                UO.PrintInformation("Morfing zapnut!");
                morfingRunning = true;
            }
        }

        private CallbackResult OnUpdatePlayer(byte[] data, CallbackResult prevResult)
        {
            PacketReader reader = new PacketReader(data);
            byte id = reader.ReadByte();
            Serial serial = reader.ReadUInt32();
            if (canMorf(data)) ByteConverter.BigEndian.ToBytes((ushort)0x0192, data, 5);
            return CallbackResult.Normal;
        }

        private bool canMorf(byte[] data)
        {
            if (World.Player.Dead) return false; // mrtvej
            else if ((data[5] == 0x10) && (data[6] == 0x69)) return false; // ghost
            else if ((data[5] == 0x01) && ((data[6] == 0x90) || (data[6] == 0x91))) return false;
            return true;
        }

        private CallbackResult OnAutoUpdateCharacter(byte[] data, CallbackResult prevResult)
        {
            PacketReader reader = new PacketReader(data);
            reader.Skip(5);
            ushort model = reader.ReadUInt16();
            if ((model == 0x0005) || (model == 0x0006)) model = 0x0004; // na gargoyla
            if ((model == 0x00CD) || (model == 0x00EE)) model = 0x002C; // na ratmana
            if (model == 0x0034) model = 0x0021; // na lizardmana
            ByteConverter.BigEndian.ToBytes((ushort)model, data, 5);
            return CallbackResult.Normal;
        }

        private CallbackResult OnUpdateCharacter(byte[] data, CallbackResult prevResult)
        {
            PacketReader reader = new PacketReader(data);
            byte id = reader.ReadByte();
            Serial serial = reader.ReadUInt32();
            if (morfedCharacterSerials.Contains(serial))
            {
                ByteConverter.BigEndian.ToBytes((ushort)morfedCharactersModels[serial], data, 5);
            }
            return CallbackResult.Normal;
        }
        [Command]
        public void morfallsnakes()
        { 
           UO.Print("Snake is BIG..");
            UOCharacter output = new UOCharacter(World.Player.Serial);
            foreach (UOCharacter ch in World.Characters)
            {
                if ((ch.Model == 0x0006 || ch.Model == 0x0034 || ch.Model == 0x0005) && (ch.Name != World.Player.Name))
                {
                    if (ch.Distance < 15 && ch.Distance > output.Distance)
                    {
                        UO.Exec("morfToid", "0x000C", ch.Serial);
                        ch.RequestStatus();
                        UO.Exec("statusbar", ch.Serial);
                    }
                }
            }
        }
         [Command]
        public void morf_mirror()
        {
        UO.Print("Mirror Original");
            UOCharacter output = new UOCharacter(World.Player.Serial);
            foreach (UOCharacter ch in World.Characters)
            {
                if ((ch.Model == 0x001C) && (ch.Name != World.Player.Name))
                {
                    if (ch.Distance < 15 && ch.Distance > output.Distance)
                    {
                        if (ch.MaxHits > 100)
                        {
                            ch.Print("!!");
                            UO.Exec("statusbar", ch.Serial);
                        }
                    }
                }
            }
        }
        
        [Command]
        public void morfallbig()
        {
            ushort model = 0x0000;
            foreach (UOCharacter chara in World.Characters)
            {
                model = chara.Model;
                if ((chara.Model == 0x0005) || (chara.Model == 0x0006)) model = 0x0004; // na gargoyla
                if ((chara.Model == 0x00CD) || (chara.Model == 0x00EE)) model = 0x002C; // na ratmana
                if (chara.Model == 0x0034) model = 0x0021; // na lizardmana
                byte[] buffer = new byte[17] { 0x77, 0x00, 0x20, 0xBF, 0x0F, 0x00, 0x07, 0x04, 0xD0, 0x05, 0x64, 0x00, 0x02, 0x05, 0x7D, 0x00, 0x06 };
                if (!morfedCharacterSerials.Contains(chara.Serial)) morfedCharacterSerials.Add(chara.Serial);
                if (!morfedCharactersModels.ContainsKey(chara.Serial)) morfedCharactersModels.Add(chara.Serial, model);
                ByteConverter.BigEndian.ToBytes(chara.Serial, buffer, 1);
                ByteConverter.BigEndian.ToBytes((ushort)model, buffer, 5);
                ByteConverter.BigEndian.ToBytes(chara.X, buffer, 7);
                ByteConverter.BigEndian.ToBytes(chara.Y, buffer, 9);
                ByteConverter.BigEndian.ToBytes(chara.Z, buffer, 11);
                ByteConverter.BigEndian.ToBytes(chara.Direction, buffer, 12);
                ByteConverter.BigEndian.ToBytes(chara.Color, buffer, 13);
                ByteConverter.BigEndian.ToBytes(chara.Flags, buffer, 15);
                ByteConverter.BigEndian.ToBytes((byte)chara.Notoriety, buffer, 16);
                Core.SendToClient(buffer);
            }
            UO.PrintInformation("Vsechno male je velke");
        }

        [Command]
        public void morfallsmall()
        {
            ushort model = 0x0000;
            foreach (UOCharacter chara in World.Characters)
            {
                model = chara.Model;
                if ((chara.Model == 0x0009) || (chara.Model == 0x000A)) model = 0x0004; // na gargoyla
                if (chara.Model == 0x000C) model = 0x002C; // na maleho draka
                byte[] buffer = new byte[17] { 0x77, 0x00, 0x20, 0xBF, 0x0F, 0x00, 0x07, 0x04, 0xD0, 0x05, 0x64, 0x00, 0x02, 0x05, 0x7D, 0x00, 0x06 };
                if (!morfedCharacterSerials.Contains(chara.Serial)) morfedCharacterSerials.Add(chara.Serial);
                if (!morfedCharactersModels.ContainsKey(chara.Serial)) morfedCharactersModels.Add(chara.Serial, model);
                ByteConverter.BigEndian.ToBytes(chara.Serial, buffer, 1);
                ByteConverter.BigEndian.ToBytes((ushort)model, buffer, 5);
                ByteConverter.BigEndian.ToBytes(chara.X, buffer, 7);
                ByteConverter.BigEndian.ToBytes(chara.Y, buffer, 9);
                ByteConverter.BigEndian.ToBytes(chara.Z, buffer, 11);
                ByteConverter.BigEndian.ToBytes(chara.Direction, buffer, 12);
                ByteConverter.BigEndian.ToBytes(chara.Color, buffer, 13);
                ByteConverter.BigEndian.ToBytes(chara.Flags, buffer, 15);
                ByteConverter.BigEndian.ToBytes((byte)chara.Notoriety, buffer, 16);
                Core.SendToClient(buffer);
            }
            UO.PrintInformation("Vsechno velke je male");
        }

        [Command]
        public void morfbig()
        {
            UO.Print("Zamer character na morf");
            UOCharacter chara = World.GetCharacter(UIManager.TargetObject());
            byte[] buffer = new byte[17] { 0x77, 0x00, 0x20, 0xBF, 0x0F, 0x00, 0x07, 0x04, 0xD0, 0x05, 0x64, 0x00, 0x02, 0x05, 0x7D, 0x00, 0x06 };
            if (!morfedCharacterSerials.Contains(chara.Serial)) morfedCharacterSerials.Add(chara.Serial);
            if (!morfedCharactersModels.ContainsKey(chara.Serial)) morfedCharactersModels.Add(chara.Serial, 0x000C);
            ByteConverter.BigEndian.ToBytes(chara.Serial, buffer, 1);
            ByteConverter.BigEndian.ToBytes((ushort)0x000C, buffer, 5);
            ByteConverter.BigEndian.ToBytes(chara.X, buffer, 7);
            ByteConverter.BigEndian.ToBytes(chara.Y, buffer, 9);
            ByteConverter.BigEndian.ToBytes(chara.Z, buffer, 11);
            ByteConverter.BigEndian.ToBytes(chara.Direction, buffer, 12);
            ByteConverter.BigEndian.ToBytes(chara.Color, buffer, 13);
            ByteConverter.BigEndian.ToBytes(chara.Flags, buffer, 15);
            ByteConverter.BigEndian.ToBytes((byte)chara.Notoriety, buffer, 16);
            Core.SendToClient(buffer);
        }

        [Command]
        public void morftarget()
        {
            UO.Print("Zamer character na morf");
            UOCharacter chara = World.GetCharacter(UIManager.TargetObject());
            byte[] buffer = new byte[17] { 0x77, 0x00, 0x20, 0xBF, 0x0F, 0x00, 0x07, 0x04, 0xD0, 0x05, 0x64, 0x00, 0x02, 0x05, 0x7D, 0x00, 0x06 };
            if (!morfedCharacterSerials.Contains(chara.Serial)) morfedCharacterSerials.Add(chara.Serial);
            if (!morfedCharactersModels.ContainsKey(chara.Serial)) morfedCharactersModels.Add(chara.Serial, 0x0123);
            ByteConverter.BigEndian.ToBytes(chara.Serial, buffer, 1);
            ByteConverter.BigEndian.ToBytes((ushort)0x0123, buffer, 5);
            ByteConverter.BigEndian.ToBytes(chara.X, buffer, 7);
            ByteConverter.BigEndian.ToBytes(chara.Y, buffer, 9);
            ByteConverter.BigEndian.ToBytes(chara.Z, buffer, 11);
            ByteConverter.BigEndian.ToBytes(chara.Direction, buffer, 12);
            ByteConverter.BigEndian.ToBytes(chara.Color, buffer, 13);
            ByteConverter.BigEndian.ToBytes(chara.Flags, buffer, 15);
            ByteConverter.BigEndian.ToBytes((byte)chara.Notoriety, buffer, 16);
            Core.SendToClient(buffer);
        }
        
         [Command]
        public void morftarget1()
        {
            UO.Print("Zamer character na morf");
            UOCharacter chara = World.GetCharacter(UIManager.TargetObject());
            byte[] buffer = new byte[17] { 0x77, 0x00, 0x20, 0xBF, 0x0F, 0x00, 0x07, 0x04, 0xD0, 0x05, 0x64, 0x00, 0x02, 0x05, 0x7D, 0x00, 0x06 };
            if (!morfedCharacterSerials.Contains(chara.Serial)) morfedCharacterSerials.Add(chara.Serial);
            if (!morfedCharactersModels.ContainsKey(chara.Serial)) morfedCharactersModels.Add(chara.Serial, 0x0122);
            ByteConverter.BigEndian.ToBytes(chara.Serial, buffer, 1);
            ByteConverter.BigEndian.ToBytes((ushort)0x0122, buffer, 5);
            ByteConverter.BigEndian.ToBytes(chara.X, buffer, 7);
            ByteConverter.BigEndian.ToBytes(chara.Y, buffer, 9);
            ByteConverter.BigEndian.ToBytes(chara.Z, buffer, 11);
            ByteConverter.BigEndian.ToBytes(chara.Direction, buffer, 12);
            ByteConverter.BigEndian.ToBytes(chara.Color, buffer, 13);
            ByteConverter.BigEndian.ToBytes(chara.Flags, buffer, 15);
            ByteConverter.BigEndian.ToBytes((byte)chara.Notoriety, buffer, 16);
            Core.SendToClient(buffer);
        }
        
       [Command]
        public void morfsmall()
        {
            UO.Print("Zamer character na morf");
            UOCharacter chara = World.GetCharacter(UIManager.TargetObject());
            byte[] buffer = new byte[17] { 0x77, 0x00, 0x20, 0xBF, 0x0F, 0x00, 0x07, 0x04, 0xD0, 0x05, 0x64, 0x00, 0x02, 0x05, 0x7D, 0x00, 0x06 };
            if (!morfedCharacterSerials.Contains(chara.Serial)) morfedCharacterSerials.Add(chara.Serial);
            if (!morfedCharactersModels.ContainsKey(chara.Serial)) morfedCharactersModels.Add(chara.Serial, 0x0005);
            ByteConverter.BigEndian.ToBytes(chara.Serial, buffer, 1);
            ByteConverter.BigEndian.ToBytes((ushort)0x0005, buffer, 5);
            ByteConverter.BigEndian.ToBytes(chara.X, buffer, 7);
            ByteConverter.BigEndian.ToBytes(chara.Y, buffer, 9);
            ByteConverter.BigEndian.ToBytes(chara.Z, buffer, 11);
            ByteConverter.BigEndian.ToBytes(chara.Direction, buffer, 12);
            ByteConverter.BigEndian.ToBytes(chara.Color, buffer, 13);
            ByteConverter.BigEndian.ToBytes(chara.Flags, buffer, 15);
            ByteConverter.BigEndian.ToBytes((byte)chara.Notoriety, buffer, 16);
            Core.SendToClient(buffer);
        }
   
        [Command]
        public void morfTo(ushort model)
        {
            UO.Print("Zamer character na morf");
            UOCharacter chara = World.GetCharacter(UIManager.TargetObject());
            byte[] buffer = new byte[17] { 0x77, 0x00, 0x20, 0xBF, 0x0F, 0x00, 0x07, 0x04, 0xD0, 0x05, 0x64, 0x00, 0x02, 0x05, 0x7D, 0x00, 0x06 };
            if (!morfedCharacterSerials.Contains(chara.Serial)) morfedCharacterSerials.Add(chara.Serial);
            if (!morfedCharactersModels.ContainsKey(chara.Serial)) morfedCharactersModels.Add(chara.Serial, model);
            ByteConverter.BigEndian.ToBytes(chara.Serial, buffer, 1);
            ByteConverter.BigEndian.ToBytes(model, buffer, 5);
            ByteConverter.BigEndian.ToBytes(chara.X, buffer, 7);
            ByteConverter.BigEndian.ToBytes(chara.Y, buffer, 9);
            ByteConverter.BigEndian.ToBytes(chara.Z, buffer, 11);
            ByteConverter.BigEndian.ToBytes(chara.Direction, buffer, 12);
            ByteConverter.BigEndian.ToBytes(chara.Color, buffer, 13);
            ByteConverter.BigEndian.ToBytes(chara.Flags, buffer, 15);
            ByteConverter.BigEndian.ToBytes((byte)chara.Notoriety, buffer, 16);
            Core.SendToClient(buffer);
        }
        
        [Executable]
          public void morfToid(ushort model, Serial sers)
        {
            UO.Print("Character byl zmenen");
            UOCharacter chara = new UOCharacter(sers);
            byte[] buffer = new byte[17] { 0x77, 0x00, 0x20, 0xBF, 0x0F, 0x00, 0x07, 0x04, 0xD0, 0x05, 0x64, 0x00, 0x02, 0x05, 0x7D, 0x00, 0x06 };
            if (!morfedCharacterSerials.Contains(chara.Serial)) morfedCharacterSerials.Add(chara.Serial);
            if (!morfedCharactersModels.ContainsKey(chara.Serial)) morfedCharactersModels.Add(chara.Serial, model);
            ByteConverter.BigEndian.ToBytes(chara.Serial, buffer, 1);
            ByteConverter.BigEndian.ToBytes(model, buffer, 5);
            ByteConverter.BigEndian.ToBytes(chara.X, buffer, 7);
            ByteConverter.BigEndian.ToBytes(chara.Y, buffer, 9);
            ByteConverter.BigEndian.ToBytes(chara.Z, buffer, 11);
            ByteConverter.BigEndian.ToBytes(chara.Direction, buffer, 12);
            ByteConverter.BigEndian.ToBytes(chara.Color, buffer, 13);
            ByteConverter.BigEndian.ToBytes(chara.Flags, buffer, 15);
            ByteConverter.BigEndian.ToBytes((byte)chara.Notoriety, buffer, 16);
            Core.SendToClient(buffer);
        }
    }
}