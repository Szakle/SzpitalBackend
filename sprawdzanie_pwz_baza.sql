-- ============================================================
-- FUNKCJA WALIDACJI PWZ - Oryginalnego numeru z pierwszej cyfry
-- ============================================================
CREATE OR REPLACE FUNCTION sprawdz_pwz_pojedynczy(pwz TEXT)
RETURNS BOOLEAN AS $$
DECLARE
    cyfra_kontrolna int;
BEGIN
    -- Sprawdzenie czy pierwsza cyfra nie jest 0
    if substr(pwz, 1, 1) = '0' then
        return false;
    end if; 
    
    -- Sprawdzenie czy PWZ zawiera dokładnie 7 cyfr
    if pwz !~ '^[0-9]{7}$' then
        return false;
    else
        -- Obliczenie cyfry kontrolnej na podstawie wag
        cyfra_kontrolna := mod(
            (int4(substr(pwz, 2, 1)) * 1)
            + (int4(substr(pwz, 3, 1)) * 2)
            + (int4(substr(pwz, 4, 1)) * 3)
            + (int4(substr(pwz, 5, 1)) * 4)
            + (int4(substr(pwz, 6, 1)) * 5)
            + (int4(substr(pwz, 7, 1)) * 6)
            , 11
        );
        
        if cyfra_kontrolna = 10 then
            cyfra_kontrolna := 0;
        end if;
        
        return text(cyfra_kontrolna) = substr(pwz, 1, 1); 
    end if; 
END;
$$ LANGUAGE plpgsql;


-- ============================================================
-- FUNKCJA SPRAWDZAJĄCA PWZ W BAZIE - dla konkretnego ID
-- ============================================================
CREATE OR REPLACE FUNCTION sprawdz_pwz_w_bazie(p_id_uprawnien INT)
RETURNS TABLE(
    id_uprawnien INT,
    npwz_id_rizh TEXT,
    czy_prawidlowa BOOLEAN,
    komunikat TEXT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        uz.id_uprawnien,
        uz.npwz_id_rizh,
        sprawdz_pwz_pojedynczy(uz.npwz_id_rizh)::BOOLEAN,
        CASE 
            WHEN sprawdz_pwz_pojedynczy(uz.npwz_id_rizh) THEN 'PWZ prawidłowy'
            WHEN substr(uz.npwz_id_rizh, 1, 1) = '0' THEN 'Błąd: pierwsza cyfra to 0'
            WHEN uz.npwz_id_rizh !~ '^[0-9]{7}$' THEN 'Błąd: PWZ musi zawierać dokładnie 7 cyfr'
            ELSE 'Błąd: cytra kontrolna nie zgadza się'
        END as komunikat
    FROM uprawnienia_zawodowe uz
    WHERE uz.id_uprawnien = p_id_uprawnien;
END;
$$ LANGUAGE plpgsql;


-- ============================================================
-- FUNKCJA SPRAWDZAJĄCA WSZYSTKIE PWZ W TABELI
-- ============================================================
CREATE OR REPLACE FUNCTION sprawdz_wszystkie_pwz_w_bazie()
RETURNS TABLE(
    id_uprawnien INT,
    npwz_id_rizh TEXT,
    czy_prawidlowa BOOLEAN,
    komunikat TEXT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        uz.id_uprawnien,
        uz.npwz_id_rizh,
        sprawdz_pwz_pojedynczy(uz.npwz_id_rizh)::BOOLEAN,
        CASE 
            WHEN sprawdz_pwz_pojedynczy(uz.npwz_id_rizh) THEN 'PWZ prawidłowy'
            WHEN substr(uz.npwz_id_rizh, 1, 1) = '0' THEN 'Błąd: pierwsza cyfra to 0'
            WHEN uz.npwz_id_rizh !~ '^[0-9]{7}$' THEN 'Błąd: PWZ musi zawierać dokładnie 7 cyfr'
            ELSE 'Błąd: cytra kontrolna nie zgadza się'
        END as komunikat
    FROM uprawnienia_zawodowe uz
    ORDER BY uz.id_uprawnien;
END;
$$ LANGUAGE plpgsql;


-- ============================================================
-- TRIGGER DO WALIDACJI PWZ PRZY WSTAWIANIU/AKTUALIZACJI
-- ============================================================
CREATE OR REPLACE FUNCTION waliduj_pwz_trigger()
RETURNS TRIGGER AS $$
BEGIN
    IF NOT sprawdz_pwz_pojedynczy(NEW.npwz_id_rizh) THEN
        RAISE EXCEPTION 'Nieprawidłowy numer PWZ: %. Cytra kontrolna nie zgadza się.', NEW.npwz_id_rizh;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Usunięcie starego triggera jeśli istnieje
DROP TRIGGER IF EXISTS sprawdz_pwz_on_insert_update ON uprawnienia_zawodowe;

-- Utworzenie triggera
CREATE TRIGGER sprawdz_pwz_on_insert_update
    BEFORE INSERT OR UPDATE ON uprawnienia_zawodowe
    FOR EACH ROW
    EXECUTE FUNCTION waliduj_pwz_trigger();


-- ============================================================
-- PRZYKŁADY UŻYCIA:
-- ============================================================

-- 1. Sprawdzenie konkretnego PWZ:
-- SELECT sprawdz_pwz_pojedynczy('1234567');

-- 2. Sprawdzenie konkretnego rekordu z bazy (np. id_uprawnien = 1):
-- SELECT * FROM sprawdz_pwz_w_bazie(1);

-- 3. Sprawdzenie wszystkich PWZ w tabeli uprawnienia_zawodowe:
-- SELECT * FROM sprawdz_wszystkie_pwz_w_bazie();

-- 4. Znalezienie wszystkich nieprawidłowych PWZ:
-- SELECT * FROM sprawdz_wszystkie_pwz_w_bazie() WHERE NOT czy_prawidlowa;
