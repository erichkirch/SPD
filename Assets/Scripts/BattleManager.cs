using System.Collections;
using System.Collections.Generic;
//using System.Threading;
using UnityEngine;
using System.Timers;
using System;

public class BattleManager : MonoBehaviour
{
    public enum FightOutcomes { KILLED, ALIVE };
    private Timer theTimer;
    Enemy enemyScript;
    Timer timer;

    public int Fight(Player player, GameObject enemy, bool playerAttack, EnemyManager enemyManagerScript)
    {
        int result = (int)FightOutcomes.ALIVE;

        // IngameUI.logPrint("FIGHT! between " + player.name + " and Enemy " + enemy.GetComponent<Enemy>().enemyID);
        // if player atacking
        if (playerAttack)
        {
            enemyScript = enemy.GetComponent<Enemy>();

            //StartTimer();
            PlayEnemyHitAnimation(enemyScript);

            enemyScript.enemyHP -= player.playerAttackDmg;
            IngameUI.logPrint("Attacked " + enemy.name +  " for " + player.playerAttackDmg);// + " and enemy HP reduced to " + enemyScript.enemyHP);

            if (enemyScript.enemyHP <= 0)
            {
                IngameUI.logPrint(enemy.name + " was killed!");
                result = (int)FightOutcomes.KILLED;
                player.grantXP(2);

                GameObject skeletonPrefab = enemyManagerScript.skeletonPrefab;
                GameObject skeletonClone = Instantiate(skeletonPrefab);
                skeletonClone.transform.SetParent(this.transform);
                skeletonClone.transform.position = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
            }
            //enemyScript.animator.speed = 0;
        }
        // if enemy attacking
        else
        {
            Player.updateHealth(-enemy.GetComponent<Enemy>().enemyAttackDmg);
            IngameUI.logPrint(enemy.name + " attacks Player for " + enemy.GetComponent<Enemy>().enemyAttackDmg);

            if (player.playerHP <= 0)
            {
                IngameUI.health.text = "DEAD";
                if(!player.dead) {
                    player.dead = true;
                    IngameUI.logPrint("You have died!");
                }
                result = (int)FightOutcomes.KILLED;
            }
        }

        return result;
    }

    //Will be included soon!
    private void PlayEnemyHitAnimation(Enemy enemyScript)
    {
        enemyScript.animator.speed = 1;

        string entityName = enemyScript.name.Replace("(Clone)", "");
        entityName = enemyScript.name.Replace(" ", "");
        // IngameUI.logPrint("Enemy type " + entityName + " previousAnimation is " + enemyScript.previousAnimation);
        if (enemyScript.previousAnimation == "down")
        {
            StartCoroutine(WaitForAnim(entityName));
            enemyScript.animator.SetTrigger(entityName + "DownHit");
        }
        if (enemyScript.previousAnimation == "left")
        {
            StartCoroutine(WaitForAnim(entityName));
            enemyScript.animator.SetTrigger(entityName + "LeftHit");
        }
        if (enemyScript.previousAnimation == "right")
        {
            StartCoroutine(WaitForAnim(entityName));
            enemyScript.animator.SetTrigger(entityName + "RightHit");
        }
        if (enemyScript.previousAnimation == "up")
        {
            StartCoroutine(WaitForAnim(entityName));
            enemyScript.animator.SetTrigger(entityName + "UpHit");
        }
        //StartCoroutine(WaitForAnim(entityName));
    }

    IEnumerator WaitForAnim(string entityName)
    {
        while ((enemyScript != null) && (!enemyScript.animator.GetCurrentAnimatorStateInfo(0).IsName(entityName + "_Hit_Down_Anim")
                        && !enemyScript.animator.GetCurrentAnimatorStateInfo(0).IsName(entityName + "_Hit_Left_Anim")
                        && !enemyScript.animator.GetCurrentAnimatorStateInfo(0).IsName(entityName + "_Hit_Up_Anim")
                        && !enemyScript.animator.GetCurrentAnimatorStateInfo(0).IsName(entityName + "_Hit_Right_Anim")))
        {
            yield return null;
        }
        while ((enemyScript != null) && (enemyScript.animator.GetCurrentAnimatorStateInfo(0).IsName(entityName + "_Hit_Down_Anim")
                        || enemyScript.animator.GetCurrentAnimatorStateInfo(0).IsName(entityName + "_Hit_Left_Anim")
                        || enemyScript.animator.GetCurrentAnimatorStateInfo(0).IsName(entityName + "_Hit_Up_Anim")
                        || enemyScript.animator.GetCurrentAnimatorStateInfo(0).IsName(entityName + "_Hit_Right_Anim")))
        {
            yield return null;
        }
        if(enemyScript.animator != null)
        {
            enemyScript.animator.speed = 0;
        }
    }
}