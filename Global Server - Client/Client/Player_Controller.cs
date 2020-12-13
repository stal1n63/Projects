using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    Client client;

    public ushort ID;
    public byte OwnerID;
    public byte AnimationState_Low;
    public byte AnimationState_Medium;
    public byte AnimationState_High;
    public ushort ModelID;
    public ushort Model_TextureID;
    public ushort WeaponID_Main;
    public ushort Weapon_TextureID_Main;
    public ushort WeaponID_Secondary;
    public ushort Weapon_TextureID_Secondary;
    public byte Health;
    public byte Speed;
    public string OwnerNickname;
    public byte LastDamagedPlayerID;
    public string LastDamagedPlayerNickname;
    public ushort LastDamagedWeaponID;
    public byte LastDamagedSlot;

    public Animator _animator;
    public GameObject _camera;
    public float mouseSensitivity = 1.8f;
    public float speed = 4f;
    public float jumpSpeed = 8f;
    public float gravity = 20f;

    public byte Team;

    CharacterController controller;
    GlobalServerListener global;
    GameObject kost;

   // PrefabIdSwapper swapper;

    public GameObject model3D;

    float vertical, horizontal, _1, _2;

    Vector3 newPosition;
    Vector3 newRotation;
    public float smoothFactor = 11f;

    Vector3 final = Vector3.zero;

    public byte selected;
    public GameObject firstSlot, secSlot, thSlot, fourthSlot;

    GameUI ui;

    private void Start()
    {
        ui = GameObject.Find("Game Canvas").GetComponent<GameUI>();

        //swapper = GameObject.Find("ServerRulers").gameObject.GetComponentInChildren<PrefabIdSwapper>();
        global = GameObject.Find("DontDestroyOnLoadInstance").GetComponent<GlobalServerListener>();
        controller = this.GetComponent<CharacterController>();
        kost = _camera.transform.parent.gameObject;

        var rend = model3D.GetComponent<SkinnedMeshRenderer>();
       // rend.sharedMesh = swapper.models[ModelID].model.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //rend.material = swapper.modelMaterials[Model_TextureID].materials;

        newPosition = transform.position;
        newRotation = transform.rotation.eulerAngles;
        
        if (OwnerID == client.playerID)
        {
            foreach (Transform child in this.transform)
            {
                child.gameObject.layer = 12;
            }

            StartCoroutine("UpdateInServerObject");
            StartCoroutine("UpdateObject");
           // firstSlot = Instantiate(swapper.weapons_main[WeaponID_Main].model, _camera.transform);
            var f = firstSlot.GetComponent<Shoot_OverNetwork>();
           // f.bullets_now = global.balance.Weapons[swapper.weapons_main[WeaponID_Main].name].Ammo_Magazine;
           // f.bullets_all = global.balance.Weapons[swapper.weapons_main[WeaponID_Main].name].Ammo_All;
            f.WeaponID = WeaponID_Main;
            f.WeaponTextureID = Weapon_TextureID_Main;
            f.HandsID = ModelID;
            f.HandsTextureID = Model_TextureID;
           // f.Damage = global.balance.Weapons[swapper.weapons_main[WeaponID_Main].name].Damage;
            f.Enable();

           // secSlot = Instantiate(swapper.weapons_sec[WeaponID_Secondary].model, _camera.transform);
            var s = secSlot.GetComponent<Shoot_OverNetwork>();
          //  s.bullets_now = global.balance.Weapons[swapper.weapons_sec[WeaponID_Secondary].name].Ammo_Magazine;
          //  s.bullets_all = global.balance.Weapons[swapper.weapons_sec[WeaponID_Secondary].name].Ammo_All;
            s.WeaponID = WeaponID_Secondary;
            s.WeaponTextureID = Weapon_TextureID_Secondary;
            s.HandsID = ModelID;
            s.HandsTextureID = Model_TextureID;
          //  s.Damage = global.balance.Weapons[swapper.weapons_sec[WeaponID_Secondary].name].Damage;
            secSlot.SetActive(false);

            selected = 0;
        }
        else
        {
            controller.enabled = false;
            _camera.SetActive(false);
            rend.gameObject.layer = 0;
        }
    }

    void Update()
    {
        _animator.SetInteger("state", AnimationState_Low);
        if (OwnerID != client.playerID)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, newPosition, Time.deltaTime * smoothFactor);
            this.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRotation), Time.deltaTime * smoothFactor);
        }
        else
        {
            horizontal += Input.GetAxis("Mouse X") * mouseSensitivity;
            vertical -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            vertical = Mathf.Clamp(vertical, -45f, 75f);

            _1 = Input.GetAxis("Horizontal");
            _2 = Input.GetAxis("Vertical");

            if (controller.isGrounded)
            {
                if (Input.GetAxis("Run") == 0)
                {
                    final = new Vector3(_1, 0, _2);
                    speed = 1.8f;
                }
                else
                {
                    if (!Input.GetKey(KeyCode.Mouse1))
                    {
                        final = new Vector3(0, 0, (Mathf.Clamp(_2, 0, 1)));
                        speed = 4f;
                    }
                    else
                    {
                        final = new Vector3(_1, 0, _2);
                        speed = 1.8f;
                    }
                }
                final = transform.TransformDirection(final);
                final *= speed;
                if (Input.GetAxis("Jump") != 0)
                {
                    final.y = jumpSpeed;
                }
            }

            //animator
            AnimationState_Low = (int)Animation_States.Idle;
            if (_2 > 0)
            {
                AnimationState_Low = (int)Animation_States.Walk_Forward;
            }
            if(_2 < 0)
            {
                AnimationState_Low = (int)Animation_States.Walk_Back;
            }
            if (_1 > 0)
            {
                AnimationState_Low = (int)Animation_States.Walk_Left;
            }
            if (_1 < 0)
            {
                AnimationState_Low = (int)Animation_States.Walk_Right;
            }


            kost.transform.localRotation = Quaternion.Euler(vertical, 0, 0);

            final.y -= (gravity * Time.deltaTime);
            controller.Move(final * Time.deltaTime);
            controller.transform.rotation = Quaternion.Euler(0, horizontal, 0);


            if (Input.GetKeyDown(KeyCode.Alpha1) & selected != 0)
            {
                selected = 0;
                Select(selected);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) & selected != 1)
            {
                selected = 1;
                Select(selected);
            }
        }
    }

    IEnumerator UpdateObject()
    {
        Vector3 _lastPos = this.transform.position;
        Vector3 _lastRot = this.transform.rotation.eulerAngles;
        yield return new WaitForSeconds(0.1f);
        if (this.transform.position != _lastPos | this.transform.rotation.eulerAngles != _lastRot)
        {
            //c_listener.UpdatePlayerTransform(this.ID, transform.position, transform.rotation.eulerAngles);
        }
        //c_listener.UpdatePlayerAnimation(this.ID, AnimationState_Low, AnimationState_Medium, AnimationState_High);
        StartCoroutine("UpdateObject");
    }

    IEnumerator UpdateInServerObject()
    {
        Vector3 _lastPos = this.transform.position;
        Vector3 _lastRot = this.transform.rotation.eulerAngles;
        yield return new WaitForSeconds(2);
        if (this.transform.position != _lastPos | this.transform.rotation.eulerAngles != _lastRot)
        {
            //c_listener.UpdatePlayerInGlobal(this.ID, transform.position, transform.rotation.eulerAngles);
        }
        StartCoroutine("UpdateInServerObject");
    }

    public void GetNewTransform(Vector3 position, Vector3 rotation)
    {
        newPosition = position;
        newRotation = rotation;
        smoothFactor = Vector3.Distance(this.transform.position, newPosition);
        if (smoothFactor < 11f)
            smoothFactor = 11f;
    }

    public void GetNewAnimation(byte animLow, byte animMed, byte animHigh)
    {
        AnimationState_Low = animLow;
        AnimationState_Medium = animMed;
        AnimationState_High = animHigh;
    }

    public void Select(byte selected)
    {
        switch (selected)
        {
            case 0:
                firstSlot.SetActive(true);
                var f = firstSlot.GetComponent<Shoot_OverNetwork>();
                secSlot.SetActive(false);
                f.Enable();
                break;
            case 1:
                firstSlot.SetActive(false);
                var s = secSlot.GetComponent<Shoot_OverNetwork>();
                secSlot.SetActive(true);
                s.Enable();
                break;
        }
    }

    public void GetDamaged(byte value, byte lastDamagedPlayerID, string lastDamagedPlayerNickname, ushort lastDamagedWeaponID, byte lastDamagedSlot)
    {
        if (OwnerID == client.playerID)
        {
            LastDamagedPlayerID = lastDamagedPlayerID;
            LastDamagedSlot = lastDamagedSlot;
            LastDamagedWeaponID = lastDamagedWeaponID;
            LastDamagedPlayerNickname = lastDamagedPlayerNickname;
            LastDamagedSlot = lastDamagedSlot;
            if (this.Health < value)
            {
                Health = 0;
                if (OwnerID == client.playerID)
                {
                    StartCoroutine("Dead");
                }
                GetRagdolled();
                firstSlot.SetActive(false);
                secSlot.SetActive(false);
                thSlot.SetActive(false);
                fourthSlot.SetActive(false);
            }
            else
            {
                Health -= value;
            }
            ui.SetHealth(Health);
        }
    }

    public void GetRagdolled()
    {
        _animator.enabled = false;
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = false;
        }
    }

    //IEnumerator Dead()
    /*{
        ui.eadMenu(LastDamagedPlayerID, LastDamagedPlayerNickname, LastDamagedWeaponID, LastDamagedSlot);
        c_listener.SendRagdolled(this.ID);
        controller.enabled = false;
        yield return new WaitForSeconds(4f);
        c_listener.SendRequestDestroyPlayerInstance(OwnerID, ID);
        ui.SpawnMenu();*/
    //}
}
