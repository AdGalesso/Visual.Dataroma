-- Index: idx_superinvestor_stock

-- DROP INDEX IF EXISTS public.idx_superinvestor_stock;

CREATE UNIQUE INDEX IF NOT EXISTS idx_superinvestor_stock
    ON public.holding USING btree
    (superinvestor_id ASC NULLS LAST, stock_code COLLATE pg_catalog."default" ASC NULLS LAST)
    WITH (deduplicate_items=True)
    TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.holding
    CLUSTER ON idx_superinvestor_stock;
