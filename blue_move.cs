using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using System.Collections;
using UnityEngine.UI;

public class blue_move : Agent
{
    Rigidbody rBody;
    public GameObject enemy;
    public GameObject enemy_nolock;
    public GameObject score;
    public Image image;
    public float lapsetime;
    public float boost;
    public float myp_x, myp_y, en1_x, en1_y, en2_x, en2_y;
    
    // 初期化時に呼ばれる
    public override void Initialize()
    {
        this.rBody = GetComponent<Rigidbody>();
    }

    // 観察取得時に呼ばれる
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Vector3.Distance(this.transform.localPosition,enemy.transform.localPosition));
        sensor.AddObservation(Vector3.Distance(this.transform.localPosition,enemy_nolock.transform.localPosition));
        sensor.AddObservation(myp_x);
        sensor.AddObservation(myp_y);
        sensor.AddObservation(en1_x);
        sensor.AddObservation(en1_y);
        sensor.AddObservation(en2_x);
        sensor.AddObservation(en2_y);
        sensor.AddObservation(lapsetime);
    }

    // 行動実行時に呼ばれる
    public override void OnActionReceived(ActionBuffers actions) {
        Vector3 dirToGo = Vector3.zero;
        int action = actions.DiscreteActions[0];

        //ブーストがある
        if (2<lapsetime && lapsetime<6)
        {
            //移動
            if (action == 1) dirToGo = transform.forward;
            if (action == 2) dirToGo = transform.forward * -1.0f;
            if (action == 3) dirToGo = transform.up;
            if (action == 4) dirToGo = transform.up * -1.0f;
            rBody.AddForce(dirToGo * 3.0f, ForceMode.VelocityChange);
            //攻撃
            if (action == 6) 
            {
                this.tag = "attack";
            }
        }
        //着地
        if (3<lapsetime)
        {
            if (action == 5) 
                {
                    lapsetime = -8.0f;
                }
        }
    }

    void Update()
    {
        transform.LookAt(enemy.transform, Vector3.forward);
        transform.Rotate(new Vector3(-180f, -180f, +90f));
        rBody.velocity = Vector3.zero;
        //ブーストの状態
        if (0<=lapsetime && lapsetime<=6)
            boost = 1.0f-lapsetime/6;

        lapsetime += 1.0f;
        image.fillAmount = boost;

        //自機の一般化した位置
        if (this.transform.localPosition.x >= 0 && this.transform.localPosition.y >= 0)
        {
            myp_x = this.transform.localPosition.x;
            myp_y = this.transform.localPosition.y;
        }
        else if (this.transform.localPosition.x < 0 && this.transform.localPosition.y < 0)
        {
            myp_x = -this.transform.localPosition.x;
            myp_y = -this.transform.localPosition.y;
        }
        else if (this.transform.localPosition.x > 0 && this.transform.localPosition.y < 0)
        {
            myp_x = -this.transform.localPosition.y;
            myp_y = this.transform.localPosition.x;
        }
        else
        {
            myp_x = this.transform.localPosition.y;
            myp_y = -this.transform.localPosition.x;
        }

        //ロック中の敵の一般化した位置
        if (enemy.transform.localPosition.x >= 0 && enemy.transform.localPosition.y >= 0)
        {
            en1_x = enemy.transform.localPosition.x;
            en1_y = enemy.transform.localPosition.y;
        }
        else if (enemy.transform.localPosition.x < 0 && enemy.transform.localPosition.y < 0)
        {
            en1_x = -enemy.transform.localPosition.x;
            en1_y = -enemy.transform.localPosition.y;
        }
        else if (enemy.transform.localPosition.x > 0 && enemy.transform.localPosition.y < 0)
        {
            en1_x = -enemy.transform.localPosition.y;
            en1_y = enemy.transform.localPosition.x;
        }
        else
        {
            en1_x = enemy.transform.localPosition.y;
            en1_y = -enemy.transform.localPosition.x;
        }

        //ロック外の敵の一般化した位置
        if (enemy_nolock.transform.localPosition.x >= 0 && enemy_nolock.transform.localPosition.y >= 0)
        {
            en2_x = enemy_nolock.transform.localPosition.x;
            en2_y = enemy_nolock.transform.localPosition.y;
        }
        else if (enemy_nolock.transform.localPosition.x < 0 && enemy_nolock.transform.localPosition.y < 0)
        {
            en2_x = -enemy_nolock.transform.localPosition.x;
            en2_y = -enemy_nolock.transform.localPosition.y;
        }
        else if (enemy_nolock.transform.localPosition.x > 0 && enemy_nolock.transform.localPosition.y < 0)
        {
            en2_x = -enemy_nolock.transform.localPosition.y;
            en2_y = enemy_nolock.transform.localPosition.x;
        }
        else
        {
            en2_x = enemy_nolock.transform.localPosition.y;
            en2_y = -enemy_nolock.transform.localPosition.x;
        }
    }

    //OnTriggerEnter関数
    //接触したオブジェクトが引数otherとして渡される
    void OnTriggerEnter(Collider other)
    {
            //接触したオブジェクトのタグ
        if (other.CompareTag("attack"))
        {
            score.tag = "win_red";
            this.AddReward(-1f);
            EndEpisode();
        }
    }

    // ヒューリスティックモードの行動決定時に呼ばれる
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;
        actions[0] = 0;
        if (Input.GetKey(KeyCode.UpArrow)) actions[0] = 1;
        if (Input.GetKey(KeyCode.DownArrow)) actions[0] = 2;
        if (Input.GetKey(KeyCode.LeftArrow)) actions[0] = 3;
        if (Input.GetKey(KeyCode.RightArrow)) actions[0] = 4;
        if (Input.GetKey(KeyCode.Space)) actions[0] = 5;
        if (Input.GetKey(KeyCode.S)) actions[0] = 6;
    }
}

//mlagents-learn config/exvs_2on1.yaml --run-id=exvs_2on1 --env=apps/exvs_2on1 --height=900 --width=1600 --force
//mlagents-learn config/exvs_2on1.yaml --run-id=exvs_2on1 --force