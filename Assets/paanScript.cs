using UnityEngine;
using System.Collections;

public class paanScript : MonoBehaviour {
    public Rigidbody2D rb;

    SpriteRenderer sr;
    Animator anim;

    string direction;
    string color;

    float jumpFromHeight;
    float speedMult = 1.0f;

    bool reachedMaxJump = false;
    bool isGrounded = true;
    bool isFalling = false;
    bool canJumpAgain = true;
    bool isSliding = false;

    enum status { jumping, running, walking, idle };

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        color = "purple";
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (rb.velocity.y < -0.1f)
        {
            isFalling = true;
        }
        else if (rb.velocity.y >= 0) {
            isFalling = false;
        }

        if (Input.GetKeyDown("q")) {
            transform.position = new Vector2(-3.3f, -4.37f);
            isGrounded = true;
            isFalling = false;
        }
        
        //if (Input.GetKeyDown("x")) {
        //    if (direction == "right")
        //    {
        //        rb.transform.position = new Vector2(rb.transform.position.x + 3.0f, rb.transform.position.y);
        //    }
        //    else if (direction == "left") {
        //        rb.transform.position = new Vector2(rb.transform.position.x - 3.0f, rb.transform.position.y);
        //    }
        //}

        if (Input.GetKeyUp("left shift"))
        {
            anim.SetBool("running", false);
            speedMult = 1.0f;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey("space") && isGrounded && !isFalling && canJumpAgain)
        {
            rb.velocity = new Vector2(rb.velocity.x, 8.0f);
            jumpFromHeight = rb.transform.position.y;
            isGrounded = false;
            canJumpAgain = false;
        }
        else if (Input.GetKey("space") && isSliding) {
            rb.velocity = new Vector2(4.0f, 16.0f);
        }
        else if (Input.GetKey("space") && !reachedMaxJump && !isFalling && !isGrounded)
        {
            canJumpAgain = false;
            reachedMaxJump = (rb.position.y - jumpFromHeight) >= 2.0f ? true : false;
            rb.velocity += new Vector2(0, 1.0f);
        }
        else if (Input.GetKeyUp("space"))
        {
            canJumpAgain = true;
            isFalling = true;
            reachedMaxJump = false;
            rb.velocity = new Vector2(rb.velocity.x, (rb.velocity.y * 0.6f));
        }

        if (Input.GetKey("right"))
        {
            sr.flipX = false;
            direction = "right";
            if (Input.GetKey("left shift"))
            {
                anim.SetBool("running", true);
                anim.SetBool("walking", false);
                speedMult = 2.6f;
            }
            else
            {
                anim.SetBool("walking", true);
                anim.SetBool("running", false);
            }
            //transform.Translate(0.09f * speedMult, 0, 0);
            rb.AddForce(Vector2.right * 18.0f);
        }
        else if (Input.GetKey("left"))
        {
            sr.flipX = true;
            direction = "left";
            if (Input.GetKey("left shift"))
            {
                anim.SetBool("running", true);
                anim.SetBool("walking", false);
                speedMult = 2.6f;
            }
            else
            {
                anim.SetBool("walking", true);
                anim.SetBool("running", false);
            }
            //transform.Translate(-0.09f * speedMult, 0, 0);
            rb.AddForce(new Vector2(-1,0) * 18.0f);
        }
        else {
            anim.SetBool("walking", false);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        isGrounded = true;
        float collPosY;
        var paanPos = rb.transform.position;

        if (coll.gameObject.tag.Contains("platform")) {
            if (coll.transform.childCount == 0) {
                collPosY = coll.transform.position.y;
            } else {
                collPosY = coll.transform.GetChild(0).transform.position.y;
            }

            if (paanPos.y > collPosY) {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                reachedMaxJump = false;
            }
        }
    }

    void OnCollisionStay2D(Collision2D coll) {
        if (coll.gameObject.tag.Contains("platform") && isFalling && (Input.GetKey("right") || Input.GetKey("left")))
        {
            rb.velocity = new Vector2(0f, rb.velocity.y * 0.85f);
            isSliding = true;
        }
        else {
            isSliding = false;
        }
    }
}
