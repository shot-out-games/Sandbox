using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum OffMeshLinkMoveMethod
{
    Teleport,
    NormalSpeed,
    Parabola,
    Curve

}

[RequireComponent(typeof(NavMeshAgent))]
public class AgentLinkMover : MonoBehaviour
{
    public OffMeshLinkMoveMethod method = OffMeshLinkMoveMethod.Parabola;
    public AnimationCurve curve = new AnimationCurve();

    public float height = 2.0f;
    public float duration = .5f;//change for greater hang time, varying enemies. further between start and end may lead to duration differences
    IEnumerator Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        Animator anim = GetComponent<Animator>();
        agent.autoTraverseOffMeshLink = false;
        while (true)
        {
            //Debug.Log("onLink " + agent.isOnOffMeshLink);

            if (agent.isOnOffMeshLink && agent.enabled)
            {
                //Debug.Log("rem " + agent.remainingDistance);

                //Vector3 worldDeltaPosition = agent.nextPosition - transform.position;
                anim.SetInteger("JumpState", 1);

                if (method == OffMeshLinkMoveMethod.NormalSpeed)
                    yield return StartCoroutine(NormalSpeed(agent));
                else if (method == OffMeshLinkMoveMethod.Parabola)
                    yield return StartCoroutine(Parabola(agent, height, duration));
                else if (method == OffMeshLinkMoveMethod.Curve)
                    yield return StartCoroutine(Curve(agent, duration));


                //if (agent.isOnOffMeshLink)
                //{
                agent.enabled = true;
                agent.CompleteOffMeshLink();
                anim.SetInteger("JumpState", 0);
                Debug.Log("jump");
                //}

            }
            yield return null;
        }
    }

    IEnumerator NormalSpeed(NavMeshAgent agent)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        while (agent.transform.position != endPos)
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * (agent.baseOffset);
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {

            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }




    IEnumerator Curve(NavMeshAgent agent, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            //Debug.Log("next0 " + agent.nextPosition);
            //Debug.Log("rem0 " + agent.remainingDistance);


            float yOffset = curve.Evaluate(normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }









}

